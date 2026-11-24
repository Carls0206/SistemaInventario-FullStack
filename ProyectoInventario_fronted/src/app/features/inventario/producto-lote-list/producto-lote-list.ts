import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Lote, ProductoLote } from '../../../core/models/lote.model';
import { Producto } from '../../../core/models/producto.model';
import { LoteService } from '../../../core/services/lote.service';
import { ProductoLoteService } from '../../../core/services/producto-lote.service';
import { ProductoService } from '../../../core/services/producto.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';
import { ProductoLoteFormDialogComponent } from '../producto-lote-form-dialog/producto-lote-form-dialog';

@Component({
  selector: 'app-producto-lote-list',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './producto-lote-list.html',
  styleUrl: './producto-lote-list.css',
})
export class ProductoLoteListComponent implements OnInit {
  private service = inject(ProductoLoteService);
  private productoService = inject(ProductoService);
  private loteService = inject(LoteService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  productoLotes: ProductoLote[] = [];
  productos: Producto[] = [];
  lotes: Lote[] = [];
  loading = false;
  error = '';
  readonly displayedColumns = ['producto', 'lote', 'fechaIngreso', 'precioUnitario', 'cantidad', 'estado', 'acciones'];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    this.productoService.getAll().subscribe({
      next: (productos) => {
        this.productos = productos;
        this.loteService.getAll().subscribe({
          next: (lotes) => {
            this.lotes = lotes;
            this.service.getAll().subscribe({
              next: (data) => {
                this.productoLotes = data;
                this.loading = false;
                this.cdr.detectChanges();
              },
              error: () => this.handleLoadError(),
            });
          },
          error: () => this.handleLoadError(),
        });
      },
      error: () => this.handleLoadError(),
    });
  }

  private handleLoadError(): void {
    this.error = 'No se pudieron cargar los datos.';
    this.loading = false;
    this.cdr.detectChanges();
  }

  nombreProducto(productoId: number): string {
    return this.productos.find((p) => p.id === productoId)?.nombre || `Producto #${productoId}`;
  }

  numeroLote(loteId: number): string {
    return this.lotes.find((l) => l.id === loteId)?.numeroLote || `Lote #${loteId}`;
  }

  openCreate(): void {
    const ref = this.dialog.open(ProductoLoteFormDialogComponent, {
      width: '720px',
      maxWidth: '95vw',
      panelClass: 'inventory-dialog-panel',
      data: { productos: this.productos, lotes: this.lotes },
    });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.snackBar.open('Entrada creada correctamente', 'Cerrar', { duration: 3000, panelClass: ['snackbar-success'] });
        this.load();
      }
    });
  }

  openEdit(item: ProductoLote): void {
    const ref = this.dialog.open(ProductoLoteFormDialogComponent, {
      width: '720px',
      maxWidth: '95vw',
      panelClass: 'inventory-dialog-panel',
      data: { item, productos: this.productos, lotes: this.lotes },
    });

    ref.afterClosed().subscribe((saved) => {
      if (saved) {
        this.snackBar.open('Entrada actualizada correctamente', 'Cerrar', { duration: 3000, panelClass: ['snackbar-success'] });
        this.load();
      }
    });
  }

  toggleEstado(item: ProductoLote): void {
    const nuevoEstado = item.estado === 'A' ? 'I' : 'A';
    this.service
      .update(item.id, {
        loteId: item.loteId,
        fechaIngreso: item.fechaIngreso ?? null,
        precioUnitario: item.precioUnitario,
        cantidad: item.cantidad,
        estado: nuevoEstado,
      })
      .subscribe({
        next: () => {
          this.snackBar.open('Estado actualizado correctamente', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-success'],
          });
          this.load();
        },
        error: () => {
          this.snackBar.open('No se pudo actualizar el estado', 'Cerrar', {
            duration: 3000,
            panelClass: ['snackbar-error'],
          });
          this.load();
        },
      });
  }

  remove(item: ProductoLote): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '95vw',
      data: {
        titulo: 'Eliminar entrada',
        mensaje: `¿Deseas eliminar esta entrada de lote para "${this.nombreProducto(item.productoId)}"?`,
        textoConfirmar: 'Eliminar',
      },
    });

    ref.afterClosed().subscribe((confirmado) => {
      if (!confirmado) return;

      this.service.delete(item.id).subscribe({
        next: () => {
          this.snackBar.open('Entrada eliminada correctamente', 'Cerrar', { duration: 3000, panelClass: ['snackbar-success'] });
          this.load();
        },
        error: () => {
          this.snackBar.open('No se pudo eliminar la entrada', 'Cerrar', { duration: 3000, panelClass: ['snackbar-error'] });
        },
      });
    });
  }
}
