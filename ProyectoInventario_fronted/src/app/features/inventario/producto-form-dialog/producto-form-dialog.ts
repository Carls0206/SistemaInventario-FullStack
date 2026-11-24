import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Producto } from '../../../core/models/producto.model';
import { ProductoService } from '../../../core/services/producto.service';

export interface ProductoFormDialogData {
  item?: Producto;
}

@Component({
  selector: 'app-producto-form-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule,
  ],
  templateUrl: './producto-form-dialog.html',
  styleUrl: './producto-form-dialog.css',
})
export class ProductoFormDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(ProductoService);
  private dialogRef = inject(MatDialogRef<ProductoFormDialogComponent>);
  private cdr = inject(ChangeDetectorRef);
  readonly data = inject<ProductoFormDialogData>(MAT_DIALOG_DATA);

  saving = false;
  error = '';
  editingId: number | null = this.data.item?.id ?? null;

  form = this.fb.group({
    codigo: ['', [Validators.required, Validators.maxLength(20)]],
    nombre: ['', [Validators.required, Validators.maxLength(50)]],
    descripcion: ['', [Validators.maxLength(200)]],
    precio: [0, [Validators.required, Validators.min(0)]],
  });

  ngOnInit(): void {
    if (this.data.item) {
      const producto = this.data.item;
      this.form.patchValue({
        codigo: producto.codigo,
        nombre: producto.nombre,
        descripcion: producto.descripcion ?? '',
        precio: producto.precio,
      });
      this.form.controls.codigo.disable();
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = '';

    if (this.editingId) {
      this.service
        .update(this.editingId, {
          nombre: this.form.controls.nombre.value!,
          descripcion: this.form.controls.descripcion.value || null,
          precio: this.form.controls.precio.value!,
          estado: this.data.item?.estado ?? 'A',
        })
        .subscribe({
          next: () => this.dialogRef.close(true),
          error: (err) => {
            this.saving = false;
            this.error = err?.error?.mensaje || err?.error?.Mensaje || 'No se pudo actualizar el producto.';
            this.cdr.detectChanges();
          },
        });
    } else {
      this.service
        .create({
          codigo: this.form.controls.codigo.value!,
          nombre: this.form.controls.nombre.value!,
          descripcion: this.form.controls.descripcion.value || null,
          precio: this.form.controls.precio.value!,
        })
        .subscribe({
          next: () => this.dialogRef.close(true),
          error: (err) => {
            this.saving = false;
            this.error =
              err?.error?.mensaje || err?.error?.Mensaje || 'No se pudo crear el producto. Verifica que el código no esté duplicado.';
            this.cdr.detectChanges();
          },
        });
    }
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
