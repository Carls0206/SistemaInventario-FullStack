export interface Producto {
  id: number;
  codigo: string;
  nombre: string;
  descripcion?: string | null;
  activo: boolean;
}

export interface ProductoCreate {
  codigo: string;
  nombre: string;
  descripcion?: string | null;
}

export interface ProductoUpdate {
  nombre: string;
  descripcion?: string | null;
  activo: boolean;
}

export interface ProductoDetalle extends Producto {
  lotes: ProductoLote[];
}

export interface ProductoLote {
  id: number;
  productoId: number;
  numeroLote: string;
  fechaIngreso?: string | null;
  precioUnitario: number;
  cantidad: number;
}

export interface ProductoLoteCreate {
  productoId: number;
  numeroLote: string;
  fechaIngreso?: string | null;
  precioUnitario: number;
  cantidad: number;
}