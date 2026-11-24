import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config/api.config';
import { Producto, ProductoCreate, ProductoDetalle, ProductoUpdate } from '../models/producto.model';

@Injectable({ providedIn: 'root' })
export class ProductoService {
  private http = inject(HttpClient);

  getAll(): Observable<Producto[]> {
    return this.http.get<Producto[]>(`${API_URL}/productos`);
  }

  getById(id: number): Observable<Producto> {
    return this.http.get<Producto>(`${API_URL}/productos/${id}`);
  }

  getDetalle(id: number): Observable<ProductoDetalle> {
    return this.http.get<ProductoDetalle>(`${API_URL}/productos/${id}/detalle`);
  }

  create(data: ProductoCreate): Observable<Producto> {
    return this.http.post<Producto>(`${API_URL}/productos`, data);
  }

  update(id: number, data: ProductoUpdate): Observable<void> {
    return this.http.put<void>(`${API_URL}/productos/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/productos/${id}`);
  }
}
