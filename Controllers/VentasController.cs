using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using L02P02_2022PM650_2022SG650.Models;
using System.Linq;

namespace L02P02_2022PM650_2022SG650.Controllers
{
    public class VentasController : Controller
    {
        private readonly libreriaDbContext _context;

        public VentasController(libreriaDbContext context)
        {
            _context = context;
        }

        // GET: Ventas/IniciarVenta
        public IActionResult IniciarVenta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IniciarVenta(clientes cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.created_at = DateTime.Now;
                _context.clientes.Add(cliente);
                _context.SaveChanges();

                var pedido = new pedido_encabezado
                {
                    id_cliente = cliente.id,
                    cantidad_libros = 0,
                    total = 0,
                    estado = "P"
                };
                _context.pedido_encabezado.Add(pedido);
                _context.SaveChanges();

                return RedirectToAction("ListadoLibros", new { idPedido = pedido.id });
            }
            return View(cliente);
        }

        public IActionResult ListadoLibros(int idPedido)
        {
            // Obtener datos como en el ejemplo de equipos (sin navegaciones)
            var listadoLibros = (from l in _context.libros
                                 join a in _context.autores on l.id_autor equals a.id
                                 where l.estado == "A"
                                 select new
                                 {
                                     l.id,
                                     l.nombre,
                                     autor = a.autor,
                                     l.precio
                                 }).ToList();

            var pedido = _context.pedido_encabezado.Find(idPedido);

            ViewData["listadoLibros"] = listadoLibros;
            ViewData["idPedido"] = idPedido;
            ViewData["total"] = pedido.total;

            return View();
        }

        [HttpPost]
        public IActionResult AgregarLibro(int idPedido, int idLibro)
        {
            var libro = _context.libros.Find(idLibro);
            if (libro != null)
            {
                _context.pedido_detalle.Add(new pedido_detalle
                {
                    id_pedido = idPedido,
                    id_libro = idLibro,
                    created_at = DateTime.Now
                });

                var pedido = _context.pedido_encabezado.Find(idPedido);
                pedido.cantidad_libros++;
                pedido.total += libro.precio;
                _context.SaveChanges();
            }

            return RedirectToAction("ListadoLibros", new { idPedido });
        }

        public IActionResult CierreVenta(int idPedido)
        {
            // Obtener datos del cliente y pedido
            var pedido = (from p in _context.pedido_encabezado
                          join c in _context.clientes on p.id_cliente equals c.id
                          where p.id == idPedido
                          select new
                          {
                              p.id,
                              p.total,
                              cliente = c
                          }).FirstOrDefault();

            // Obtener detalles del pedido
            var detalles = (from d in _context.pedido_detalle
                            join l in _context.libros on d.id_libro equals l.id
                            join a in _context.autores on l.id_autor equals a.id
                            where d.id_pedido == idPedido
                            select new
                            {
                                l.nombre,
                                a.autor,
                                l.precio
                            }).ToList();

            ViewData["pedido"] = pedido;
            ViewData["detalles"] = detalles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CerrarVenta(int idPedido)
        {
            var pedido = _context.pedido_encabezado.Find(idPedido);
            if (pedido != null)
            {
                pedido.estado = "C";
                _context.SaveChanges();
                TempData["Mensaje"] = "Venta cerrada exitosamente";
            }
            return RedirectToAction("IniciarVenta");
        }
    }
}
