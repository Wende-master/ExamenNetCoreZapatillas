using ExamenNetCoreZapatillas.Models;
using ExamenNetCoreZapatillas.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ExamenNetCoreZapatillas.Controllers
{
    public class ZapatillasController : Controller
    {
        private RepositoryZapatillas repo;

        public ZapatillasController(RepositoryZapatillas repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Zapas()
        {
            List<Zapatilla> zapatillas = await this.repo.GetZapatillasAsync();
            return View(zapatillas);
        }

        public async Task<IActionResult> Detalles(int? posicion, int idzapatilla)
        {
            if(posicion ==  null)
            {
                posicion = 1;
            }
            ModelPaginacionZapas model = await this.repo.GetPaginacionZapasAsync(posicion.Value, idzapatilla);
            
            int numeroRegistros = model.NumeroRegistros;
            int siguiente = posicion.Value + 1;
            if (siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["ZAPAS"] = model.Zapatilla;
            ViewData["POSICION"] = posicion;
            return View();
        }
        public async Task<IActionResult> _ImagenesZapatillas(int? posicion, int idzapatilla)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            ModelPaginacionZapas model = await this.repo.GetPaginacionZapasAsync(posicion.Value, idzapatilla);

            int numeroRegistros = model.NumeroRegistros;
            int siguiente = posicion.Value + 1;
            if (siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }
            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["ZAPAS"] = model.Zapatilla;
            ViewData["POSICION"] = posicion;
            return PartialView("_ImagenesZapatillas", model.ImagenZapatilla);
        }


        public async Task<IActionResult> PostImagenes()
        {
            List<Zapatilla> zapatillas = await this.repo.GetZapatillasAsync();
            return View(zapatillas);
        }

        [HttpPost]
        public async Task<IActionResult> PostImagenes
            (List<string> imagenes, int idzapatilla)
        {
            await this.repo.InsertarImagenAsync(imagenes, idzapatilla);
            return RedirectToAction("Detalles", new { idzapatilla = idzapatilla });
        }
    }
}
