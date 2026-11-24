import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { ProductoLote } from '../../../core/models/lote.model';
import { Producto } from '../../../core/models/producto.model';
import { Lote } from '../../../core/models/lote.model';
import { ProductoLoteService } from '../../../core/services/producto-lote.service';

export interface ProductoLoteFormDialogData {
  item?: ProductoLote;
  productos: Producto[];
  lotes: Lote[];
}

@Component({
  selector: 'app-producto-lote-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
  ],
  templateUrl: './producto-lote-form-dialog.html',
  styleUrl: './producto-lote-form-dialog.css',
})
export class ProductoLoteFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(ProductoLoteService);
  private dialogRef = inject(MatDialogRef<ProductoLoteFormDialogComponent>);
  private cdr = inject(ChangeDetectorRef);
  readonly data = inject<ProductoLoteFormDialogData>(MAT_DIALOG_DATA);
  readonly productosDisponibles = this.data.productos.filter(
    (p) => p.estado === 'A' || p.id === this.data.item?.productoId,
  );
  readonly lotesDisponibles = this.data.lotes.filter(
    (l) => l.estado === 'A' || l.id === this.data.item?.loteId,
  );

  saving = false;
  error = '';
  editingId: number | null = this.data.item?.id ?? null;
  readonly minDate = (() => {
    const date = new Date();
    date.setHours(0, 0, 0, 0);
    return date;
  })();

  form = this.fb.group({
    productoId: [null as number | null, [Validators.required]],
    loteId: [null as number | null, [Validators.required]],
    fechaIngreso: [new Date(), [Validators.required]],
    precioUnitario: [null as number | null, [Validators.required, Validators.min(0)]],
    cantidad: [null as number | null, [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    this.form.controls.productoId.valueChanges.subscribe((productoId) => {
      if (this.editingId || !productoId) return;
      const producto = this.data.productos.find((p) => p.id === productoId);
      if (producto) {
        this.form.controls.precioUnitario.setValue(producto.precio);
      }
    });

    if (this.data.item) {
      const item = this.data.item;
      this.form.patchValue({
        productoId: item.productoId,
        loteId: item.loteId,
        fechaIngreso: item.fechaIngreso ? new Date(item.fechaIngreso) : null,
        precioUnitario: item.precioUnitario,
        cantidad: item.cantidad,
      });
      this.form.controls.productoId.disable();
    }
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
      this.dialogRef.close(true);
    };

    const onError = (err: any) => {
      this.saving = false;
      this.error = err?.error?.mensaje || err?.error?.Mensaje || 'No se pudo guardar la entrada.';
      this.cdr.detectChanges();
    };

    if (this.editingId) {
      this.service
        .update(this.editingId, {
          loteId: value.loteId!,
          fechaIngreso,
          precioUnitario: value.precioUnitario!,
          cantidad: value.cantidad!,
          estado: this.data.item?.estado ?? 'A',
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

  cancel(): void {
    this.dialogRef.close(false);
  }
}
