import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { Lote } from '../../../core/models/lote.model';
import { LoteService } from '../../../core/services/lote.service';
import { ConfirmDialogComponent } from '../../../shared/confirm-dialog/confirm-dialog';

@Component({
  selector: 'app-lote-list',
  standalone: true,
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatSnackBarModule,
    MatTableModule,
  ],
  templateUrl: './lote-list.html',
  styleUrl: './lote-list.css',
})
export class LoteListComponent implements OnInit {
  private service = inject(LoteService);
  private fb = inject(FormBuilder);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  lotes: Lote[] = [];
  loading = false;
  saving = false;
  error = '';
  editingId: number | null = null;
  readonly displayedColumns = ['numeroLote', 'fechaCreacion', 'estado', 'acciones'];

  form = this.fb.group({
    numeroLote: ['', [Validators.required, Validators.maxLength(50)]],
    estado: ['A', [Validators.required]],
  });

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';

    this.service.getAll().subscribe({
      next: (data) => {
        this.lotes = data;
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.error = 'No se pudieron cargar los lotes.';
        this.loading = false;
        this.cdr.detectChanges();
      },
    });
  }

  edit(lote: Lote): void {
    this.editingId = lote.id;
    this.form.patchValue({ numeroLote: lote.numeroLote, estado: lote.estado });
  }

  cancelEdit(): void {
    this.editingId = null;
    this.form.reset({ numeroLote: '', estado: 'A' });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.error = '';

    const onSuccess = () => {
      this.saving = false;
      this.snackBar.open(
        this.editingId ? 'Lote actualizado correctamente' : 'Lote creado correctamente',
        'Cerrar',
        { duration: 3000 },
      );
      this.cancelEdit();
      this.load();
    };

    const onError = (err: any) => {
      this.saving = false;
      this.error = err?.error?.mensaje || 'No se pudo guardar el lote.';
      this.cdr.detectChanges();
    };

    if (this.editingId) {
      this.service
        .update(this.editingId, {
          numeroLote: this.form.controls.numeroLote.value!,
          estado: this.form.controls.estado.value!,
        })
        .subscribe({ next: onSuccess, error: onError });
    } else {
      this.service
        .create({ numeroLote: this.form.controls.numeroLote.value! })
        .subscribe({ next: onSuccess, error: onError });
    }
  }

  remove(lote: Lote): void {
    const ref = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      maxWidth: '95vw',
      data: {
        titulo: 'Eliminar lote',
        mensaje: `¿Deseas eliminar el lote "${lote.numeroLote}"?`,
        textoConfirmar: 'Eliminar',
      },
    });

    ref.afterClosed().subscribe((confirmado) => {
      if (!confirmado) return;

      this.service.delete(lote.id).subscribe({
        next: () => {
          this.snackBar.open('Lote eliminado correctamente', 'Cerrar', { duration: 3000 });
          this.load();
        },
        error: () => {
          this.snackBar.open('No se pudo eliminar el lote', 'Cerrar', { duration: 3000 });
        },
      });
    });
  }
}
