import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { ProductoService } from '../../../core/services/producto.service';
import { Producto } from '../../../core/models/producto.model';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-producto-list',
  standalone: true,
  imports: [
    DecimalPipe,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './producto-list.html',
  styleUrl: './producto-list.css',
})
export class ProductoListComponent implements OnInit {
  private service = inject(ProductoService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  productos: Producto[] = [];
  loading = false;
  error = '';
  readonly displayedColumns = ['codigo', 'nombre', 'descripcion', 'precio', 'estado', 'acciones'];

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    this.service.getAll().subscribe({
      next: (data) => {
        this.productos = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudieron cargar los productos.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  remove(producto: Producto): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '95vw',
      data: {
        titulo: 'Eliminar producto',
        mensaje: `¿Deseas eliminar "${producto.nombre}"?`,
        textoConfirmar: 'Eliminar',
      },
    });

    ref.afterClosed().subscribe((confirmado) => {
      if (!confirmado) return;

      this.service.delete(producto.id).subscribe({
        next: () => {
          this.snackBar.open('Producto eliminado correctamente', 'Cerrar', { duration: 3000 });
          this.load();
        },
        error: () => {
          this.snackBar.open('No se pudo eliminar el producto', 'Cerrar', { duration: 3000 });
        },
      });
    });
  }
}
