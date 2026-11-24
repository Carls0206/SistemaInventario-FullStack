import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { LayoutComponent } from './layout/layout';
import { ProductoListComponent } from './features/inventario/producto-list/producto-list';
import { ProductoFormComponent } from './features/inventario/producto-form/producto-form';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'productos', component: ProductoListComponent },
      { path: 'productos/nuevo', component: ProductoFormComponent },
      { path: 'productos/:id/editar', component: ProductoFormComponent },
      { path: '', redirectTo: 'productos', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'login' },
];