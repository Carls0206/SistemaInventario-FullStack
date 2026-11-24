export interface Lote {
  id: number;
  numeroLote: string;
  fechaCreacion?: string | null;
  estado: string;
}

export interface LoteCreate {
  numeroLote: string;
}

export interface LoteUpdate {
  numeroLote: string;
  estado: string;
}

export interface ProductoLote {
  id: number;
  productoId: number;
  loteId: number;
  fechaIngreso?: string | null;
  precioUnitario: number;
  cantidad: number;
  estado: string;
}

export interface ProductoLoteCreate {
  productoId: number;
  loteId: number;
  fechaIngreso?: string | null;
  precioUnitario: number;
  cantidad: number;
}

export interface ProductoLoteUpdate {
  loteId: number;
  fechaIngreso?: string | null;
  precioUnitario: number;
  cantidad: number;
  estado: string;
}
