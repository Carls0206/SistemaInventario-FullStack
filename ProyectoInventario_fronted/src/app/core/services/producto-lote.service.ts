import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config/api.config';
import { ProductoLote, ProductoLoteCreate, ProductoLoteUpdate } from '../models/lote.model';

@Injectable({ providedIn: 'root' })
export class ProductoLoteService {
  private http = inject(HttpClient);

  getAll(): Observable<ProductoLote[]> {
    return this.http.get<ProductoLote[]>(`${API_URL}/producto-lotes`);
  }

  getByProducto(productoId: number): Observable<ProductoLote[]> {
    return this.http.get<ProductoLote[]>(`${API_URL}/producto-lotes/producto/${productoId}`);
  }

  getById(id: number): Observable<ProductoLote> {
    return this.http.get<ProductoLote>(`${API_URL}/producto-lotes/${id}`);
  }

  create(data: ProductoLoteCreate): Observable<ProductoLote> {
    return this.http.post<ProductoLote>(`${API_URL}/producto-lotes`, data);
  }

  update(id: number, data: ProductoLoteUpdate): Observable<void> {
    return this.http.put<void>(`${API_URL}/producto-lotes/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/producto-lotes/${id}`);
  }
}
