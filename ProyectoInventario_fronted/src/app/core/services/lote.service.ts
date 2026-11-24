import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../config/api.config';
import { Lote, LoteCreate, LoteUpdate } from '../models/lote.model';

@Injectable({ providedIn: 'root' })
export class LoteService {
  private http = inject(HttpClient);

  getAll(): Observable<Lote[]> {
    return this.http.get<Lote[]>(`${API_URL}/lotes`);
  }

  getById(id: number): Observable<Lote> {
    return this.http.get<Lote>(`${API_URL}/lotes/${id}`);
  }

  create(data: LoteCreate): Observable<Lote> {
    return this.http.post<Lote>(`${API_URL}/lotes`, data);
  }

  update(id: number, data: LoteUpdate): Observable<void> {
    return this.http.put<void>(`${API_URL}/lotes/${id}`, data);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${API_URL}/lotes/${id}`);
  }
}
