import { RenderMode, ServerRoute } from '@angular/ssr';

export const serverRoutes: ServerRoute[] = [
  { path: 'productos/:id/editar', renderMode: RenderMode.Client },
  { path: '**', renderMode: RenderMode.Client },
];