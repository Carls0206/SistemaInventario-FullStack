import { ProductoLote } from './lote.model';

export interface Producto {
  id: number;
  codigo: string;
  nombre: string;
  descripcion?: string | null;
  precio: number;
  estado: string;
}

export interface ProductoCreate {
  codigo: string;
  nombre: string;
  descripcion?: string | null;
  precio: number;
}

export interface ProductoUpdate {
  nombre: string;
  descripcion?: string | null;
  precio: number;
  estado: string;
}

export interface ProductoDetalle extends Producto {
  lotes: ProductoLote[];
}
