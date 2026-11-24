import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { ProductoService } from '../../../core/services/producto.service';
import { ProductoCreate, ProductoUpdate } from '../../../core/models/producto.model';

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
  ],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(ProductoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  id: number | null = null;
  loading = false;
  error = '';

  form = this.fb.group({
    codigo: ['', [Validators.required, Validators.maxLength(20)]],
    nombre: ['', [Validators.required, Validators.maxLength(50)]],
    descripcion: ['', [Validators.maxLength(200)]],
    precio: [0, [Validators.required, Validators.min(0)]],
    estado: ['A', [Validators.required]],
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.id = Number(idParam);
      this.form.controls.codigo.disable();
      this.load(this.id);
    }
  }

  load(id: number): void {
    this.service.getById(id).subscribe({
      next: (producto) => {
        this.form.patchValue({
          codigo: producto.codigo,
          nombre: producto.nombre,
          descripcion: producto.descripcion ?? '',
          precio: producto.precio,
          estado: producto.estado,
        });
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudo cargar el producto.';
        this.cdr.detectChanges();
      },
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';

    if (this.id) {
      const data: ProductoUpdate = {
        nombre: this.form.controls.nombre.value!,
        descripcion: this.form.controls.descripcion.value || null,
        precio: this.form.controls.precio.value!,
        estado: this.form.controls.estado.value!,
      };

      this.service.update(this.id, data).subscribe({
        next: () => {
          this.loading = false;
          this.snackBar.open('Producto actualizado correctamente', 'Cerrar', { duration: 3000 });
          this.router.navigate(['/productos']);
        },
        error: () => {
          this.loading = false;
          this.error = 'No se pudo actualizar el producto.';
          this.cdr.detectChanges();
        },
      });
    } else {
      const data: ProductoCreate = {
        codigo: this.form.controls.codigo.value!,
        nombre: this.form.controls.nombre.value!,
        descripcion: this.form.controls.descripcion.value || null,
        precio: this.form.controls.precio.value!,
      };

      this.service.create(data).subscribe({
        next: () => {
          this.loading = false;
          this.snackBar.open('Producto creado correctamente', 'Cerrar', { duration: 3000 });
          this.router.navigate(['/productos']);
        },
        error: () => {
          this.loading = false;
          this.error = 'No se pudo crear el producto. Verifica que el código no esté duplicado.';
          this.cdr.detectChanges();
        },
      });
    }
  }
}
