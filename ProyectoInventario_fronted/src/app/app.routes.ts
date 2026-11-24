import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Register } from './features/auth/register/register';
import { LayoutComponent } from './layout/layout';
import { ProductoListComponent } from './features/inventario/producto-list/producto-list';
import { LoteListComponent } from './features/inventario/lote-list/lote-list';
import { ProductoLoteListComponent } from './features/inventario/producto-lote-list/producto-lote-list';
import { authGuard } from './core/guards/auth-guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: 'productos', component: ProductoListComponent },
      { path: 'lotes', component: LoteListComponent },
      { path: 'producto-lotes', component: ProductoLoteListComponent },
      { path: '', redirectTo: 'productos', pathMatch: 'full' },
    ],
  },
  { path: '**', redirectTo: 'login' },
];
