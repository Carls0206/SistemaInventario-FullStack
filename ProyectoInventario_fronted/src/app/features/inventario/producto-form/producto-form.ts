import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ProductoService } from '../../../core/services/producto.service';
import { ProductoCreate, ProductoUpdate } from '../../../core/models/producto.model';

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.css',
})
export class ProductoFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private service = inject(ProductoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  id: number | null = null;
  loading = false;
  error = '';

  form = this.fb.group({
    codigo: ['', [Validators.required, Validators.maxLength(20)]],
    nombre: ['', [Validators.required, Validators.maxLength(50)]],
    descripcion: [''],
    activo: [true],
  });

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.id = Number(idParam);
      this.load(this.id);
      this.form.controls.codigo.disable();
    }
  }

  load(id: number): void {
    this.service.getById(id).subscribe({
      next: (p) => {
        this.form.patchValue({
          codigo: p.codigo,
          nombre: p.nombre,
          descripcion: p.descripcion ?? '',
          activo: p.activo,
        });
      },
      error: () => (this.error = 'No se pudo cargar el producto'),
    });
  }

  submit(): void {
    if (this.form.invalid) return;
    this.loading = true;
    this.error = '';

    if (this.id) {
      const data: ProductoUpdate = {
        nombre: this.form.value.nombre!,
        descripcion: this.form.value.descripcion || null,
        activo: !!this.form.value.activo,
      };
      this.service.update(this.id, data).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/productos']);
        },
        error: () => {
          this.loading = false;
          this.error = 'No se pudo actualizar';
        },
      });
    } else {
      const data: ProductoCreate = {
        codigo: this.form.value.codigo!,
        nombre: this.form.value.nombre!,
        descripcion: this.form.value.descripcion || null,
      };
      this.service.create(data).subscribe({
        next: () => {
          this.loading = false;
          this.router.navigate(['/productos']);
        },
        error: () => {
          this.loading = false;
          this.error = 'No se pudo crear (código duplicado?)';
        },
      });
    }
  }
}