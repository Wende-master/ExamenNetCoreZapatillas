using ExamenNetCoreZapatillas.Data;
using ExamenNetCoreZapatillas.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ExamenNetCoreZapatillas.Repositories
{
    #region PROCEDURES
    /*
     * CREATE PROCEDURE SP_GRUPO_ZAPAS_OUT
    (@POSICION INT, @IDPRODUCTO INT, @REGISTROS INT OUT)
    AS
    SELECT @REGISTROS = COUNT(IDIMAGEN)
    FROM IMAGENESZAPASPRACTICA
    WHERE IDPRODUCTO = @IDPRODUCTO
    SELECT IDIMAGEN, IDPRODUCTO, IMAGEN
    FROM
    (
        SELECT CAST(ROW_NUMBER() OVER (ORDER BY IDPRODUCTO) AS INT) AS POSICION,
        ISNULL(IDIMAGEN, 0) AS IDIMAGEN, IDPRODUCTO, IMAGEN
        FROM IMAGENESZAPASPRACTICA
        WHERE IDPRODUCTO = @IDPRODUCTO
    )
    AS QUERY
    WHERE QUERY.POSICION = @POSICION
    */
    #endregion
    public class RepositoryZapatillas
    {
        private ZapasContext context;

        public RepositoryZapatillas(ZapasContext context)
        {
            this.context = context;
        }

        public async Task<List<Zapatilla>> GetZapatillasAsync()
        {
            return await this.context.Zapatillas.ToListAsync();
        }

        public async Task<ModelPaginacionZapas> GetPaginacionZapasAsync(int posicion, int idProducto)
        {
            ModelPaginacionZapas model = new ModelPaginacionZapas();
                model.Zapatilla = await this.context.Zapatillas.FirstOrDefaultAsync(
                z => z.IdProducto == idProducto);
            string sql = "SP_GRUPO_ZAPAS_OUT @POSICION, @IDPRODUCTO, @REGISTROS OUT";

            SqlParameter pamPosicion = new SqlParameter("@POSICION", posicion);
            SqlParameter pamIdProducto = new SqlParameter("@IDPRODUCTO", idProducto);
            SqlParameter paramRegistros = new SqlParameter("@REGISTROS", -1);
            paramRegistros.Direction = ParameterDirection.Output;

            var consulta = this.context.ImagenZapatillas.FromSqlRaw(sql, pamPosicion, pamIdProducto, paramRegistros);
            List<ImagenZapatilla> imagenZapas =  await consulta.ToListAsync();
            model.ImagenZapatilla = imagenZapas.FirstOrDefault();
            model.NumeroRegistros = (int)paramRegistros.Value;
            return model;
        }

        public async Task InsertarImagenAsync(List<string> imagenes, int idzapatilla)
        {
            foreach (string imagen in imagenes)
            {
                int idimagen = await GetMaxIdImagenAsync();
                await this.context.ImagenZapatillas.AddAsync(
                    new ImagenZapatilla
                    {
                        IdImagen = idimagen,
                        IdProducto = idzapatilla,
                        Imagen = imagen
                    }
                );
                await this.context.SaveChangesAsync();
            }

        }

        public async Task<Zapatilla> FindZapatillaAsync(int idzapatilla)
        {
            return await this.context.Zapatillas
                .FirstOrDefaultAsync(z => z.IdProducto == idzapatilla);
        }

        private async Task<int> GetMaxIdImagenAsync()
        {
            if(this.context.ImagenZapatillas.Count() == 0)
            {
                return 1;
            }
            else
            {
                return await this.context.ImagenZapatillas.MaxAsync(z => z.IdImagen) + 1;
            }
        }
    }
}
