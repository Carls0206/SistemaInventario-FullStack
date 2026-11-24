import { ChangeDetectorRef, Component, inject } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
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
    MatSnackBarModule,
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private cdr = inject(ChangeDetectorRef);

  loading = false;
  error = '';
  hidePassword = true;
  hideConfirm = true;

  private passwordsIguales = (group: AbstractControl): ValidationErrors | null => {
    const password = group.get('password')?.value;
    const confirmar = group.get('confirmarPassword')?.value;
    return password === confirmar ? null : { passwordsNoCoinciden: true };
  };

  form = this.fb.group(
    {
      nombre: ['', [Validators.required, Validators.maxLength(50)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmarPassword: ['', [Validators.required]],
    },
    { validators: this.passwordsIguales },
  );

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';
    const { nombre, email, password } = this.form.getRawValue();

    this.auth.register({ nombre: nombre!, email: email!, password: password! }).subscribe({
      next: () => {
        this.loading = false;
        this.snackBar.open('Usuario registrado correctamente', 'Cerrar', { duration: 3000, panelClass: ['snackbar-success'] });
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.loading = false;
        this.error =
          err?.status === 409
            ? 'El correo ya está registrado.'
            : 'No se pudo registrar el usuario. Inténtalo nuevamente.';
        this.cdr.detectChanges();
      },
    });
  }
}
