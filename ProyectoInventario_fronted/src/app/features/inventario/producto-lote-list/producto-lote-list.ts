import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Lote, ProductoLote } from '../../../core/models/lote.model';
import { Producto } from '../../../core/models/producto.model';
import { LoteService } from '../../../core/services/lote.service';
import { ProductoLoteService } from '../../../core/services/producto-lote.service';
import { ProductoService } from '../../../core/services/producto.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-producto-lote-list',
  standalone: true,
  imports: [
    DatePipe,
    DecimalPipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
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
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  productoLotes: ProductoLote[] = [];
  productos: Producto[] = [];
  lotes: Lote[] = [];
  loading = false;
  saving = false;
  error = '';
  editingId: number | null = null;
  readonly displayedColumns = ['producto', 'lote', 'fechaIngreso', 'precioUnitario', 'cantidad', 'estado', 'acciones'];

  form = this.fb.group({
    productoId: [null as number | null, [Validators.required]],
    loteId: [null as number | null, [Validators.required]],
    fechaIngreso: [null as Date | null],
    precioUnitario: [null as number | null, [Validators.required, Validators.min(0)]],
    cantidad: [null as number | null, [Validators.required, Validators.min(0)]],
    estado: ['A', [Validators.required]],
  });

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

  edit(item: ProductoLote): void {
    this.editingId = item.id;
    this.form.patchValue({
      productoId: item.productoId,
      loteId: item.loteId,
      fechaIngreso: item.fechaIngreso ? new Date(item.fechaIngreso) : null,
      precioUnitario: item.precioUnitario,
      cantidad: item.cantidad,
      estado: item.estado,
    });
    this.form.controls.productoId.disable();
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form.reset({
      productoId: null,
      loteId: null,
      fechaIngreso: null,
      precioUnitario: null,
      cantidad: null,
      estado: 'A',
    });
    this.form.controls.productoId.enable();
  }

  private formatDate(value: Date | null): string | null {
    if (!value) return null;
    const year = value.getFullYear();
    const month = String(value.getMonth() + 1).padStart(2, '0');
    const day = String(value.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = '';
    const value = this.form.getRawValue();
    const fechaIngreso = this.formatDate(value.fechaIngreso);

    const onSuccess = () => {
      this.saving = false;
      this.snackBar.open(
        this.editingId ? 'Entrada actualizada correctamente' : 'Entrada creada correctamente',
        'Cerrar',
        { duration: 3000 },
      );
      this.cancelEdit();
      this.load();
    };

    const onError = (err: any) => {
      this.saving = false;
      this.error = err?.error?.mensaje || 'No se pudo guardar la entrada.';
      this.cdr.detectChanges();
    };

    if (this.editingId) {
      this.service
        .update(this.editingId, {
          loteId: value.loteId!,
          fechaIngreso,
          precioUnitario: value.precioUnitario!,
          cantidad: value.cantidad!,
          estado: value.estado!,
        })
        .subscribe({ next: onSuccess, error: onError });
    } else {
      this.service
        .create({
          productoId: value.productoId!,
          loteId: value.loteId!,
          fechaIngreso,
          precioUnitario: value.precioUnitario!,
          cantidad: value.cantidad!,
        })
        .subscribe({ next: onSuccess, error: onError });
    }
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
          this.snackBar.open('Entrada eliminada correctamente', 'Cerrar', { duration: 3000 });
          this.load();
        },
        error: () => {
          this.snackBar.open('No se pudo eliminar la entrada', 'Cerrar', { duration: 3000 });
        },
      });
    });
  }
}
