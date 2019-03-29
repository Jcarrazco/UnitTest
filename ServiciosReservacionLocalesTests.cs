using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reservaciones.Entidades;
using Reservaciones.Servicios;
using NUnit.Framework;

namespace Reservaciones.UnitTests
{
    [TestFixture]
    public class ServiciosReservacionLocalesTests
    {
        #region FiltrarBusquedaDeHabitaciones
        [Test]
        public void FiltrarBusquedaDeHabitaciones_NoEncontrado()
        {
            //Arrange
            var servicio = new ServiciosReservacionLocales();
            var hotel = new Hotel() {Categoria = Categoria.CincoEstrellas ,
                Amenidades = new List<Amenidad> { Amenidad.AguaCaliente } };

            var habitacionesDisp = new List<Habitacion> {
                                    new Habitacion{Id=11, Tipo = TipoHbitacion.Doble, Hotel = hotel} };

            var Tipos = new List<TipoHbitacion>();
            var Amenidades = new List<Amenidad>();
            var Categories = new List<Categoria>();

            //Act
            var resultado = servicio.FiltrarBusquedaDeHabitaciones(
                habitacionesDisp, Tipos, Amenidades, Categories);
            //Assert
            Assert.AreEqual(new List<Habitacion>(), resultado );

        }

        [Test]
        public void FiltrarBusquedaDeHabitaciones_TiposDiferentes()
        {
            //Arrange
            var servicio = new ServiciosReservacionLocales();
            var hotel = new Hotel()
            {
                Categoria = Categoria.CincoEstrellas,
                Amenidades = new List<Amenidad> { Amenidad.AguaCaliente }
            };
            var habitac = new Habitacion { Id = 5, Tipo = TipoHbitacion.Doble, Hotel = hotel };
            var habitacionesDisp = new List<Habitacion> {
                                    habitac,
                                    new Habitacion{Id=11, Tipo = TipoHbitacion.Doble, Hotel = hotel} };

            var Tipos = new List<TipoHbitacion>() {TipoHbitacion.Doble };
            var Amenidades = new List<Amenidad>();
            var Categories = new List<Categoria>();

            //Act
            var resultado = servicio.FiltrarBusquedaDeHabitaciones(
                habitacionesDisp, Tipos, Amenidades, Categories);
            //Assert
            Assert.IsTrue(resultado.Contains(habitac));
        }

        [Test]
        public void FiltrarBusquedaDeHabitaciones_AmenidadesDiferentes()
        {
            //Arrange
            var servicio = new ServiciosReservacionLocales();
            var hotel = new Hotel()
            {
                Categoria = Categoria.CincoEstrellas,
                Amenidades = new List<Amenidad> { Amenidad.AguaCaliente }
            };
            var habitac = new Habitacion { Id = 5, Tipo = TipoHbitacion.Doble, Hotel = hotel };
            var habitacionesDisp = new List<Habitacion> {
                                    habitac,
                                    new Habitacion{Id=11, Tipo = TipoHbitacion.Doble, Hotel = hotel} };

            var Tipos = new List<TipoHbitacion>();
            var Amenidades = new List<Amenidad>() { Amenidad.AguaCaliente};
            var Categories = new List<Categoria>();

            //Act
            var resultado = servicio.FiltrarBusquedaDeHabitaciones(
                habitacionesDisp, Tipos, Amenidades, Categories);
            //Assert
            Assert.IsTrue(resultado.Contains(habitac));
        }

        [Test]
        public void FiltrarBusquedaDeHabitaciones_CategoriasDiferentes()
        {
            //Arrange
            var servicio = new ServiciosReservacionLocales();
            var hotel = new Hotel()
            {
                Categoria = Categoria.CincoEstrellas,
                Amenidades = new List<Amenidad> { Amenidad.AguaCaliente }
            };
            var habitac = new Habitacion { Id = 5, Tipo = TipoHbitacion.Doble, Hotel = hotel };
            var habitacionesDisp = new List<Habitacion> {
                                    habitac,
                                    new Habitacion{Id=11, Tipo = TipoHbitacion.Doble, Hotel = hotel} };

            var Tipos = new List<TipoHbitacion>();
            var Amenidades = new List<Amenidad>();
            var Categories = new List<Categoria>() { Categoria.CincoEstrellas};

            //Act
            var resultado = servicio.FiltrarBusquedaDeHabitaciones(
                habitacionesDisp, Tipos, Amenidades, Categories);
            //Assert
            Assert.IsTrue(resultado.Contains(habitac));
        }
        #endregion

        #region GenerarPrecioDeHabitacionPorFechaYUsuario
        [Test]
        public void GenerarPrecioDeHabitacionPorFechaYUsuario_DescuentoPremium()
        {
            //si recibimos un usuario y es un usuario premium, aplicar descuento de 10%
            //Arrange
            var usuario = new Usuario {Id=676 , EsUsuarioPremium = true };
            var habitacion = new Habitacion {Id=4545, TarifaBase = 500m };
            var dia = new DateTime(20000901);
            var servicio = new ServiciosReservacionLocales();
            //Act
            var resultado = servicio.GenerarPrecioDeHabitacionPorFechaYUsuario(
                habitacion, dia, usuario);
            //Assert
            Assert.AreEqual(450m, resultado.Precio);
        }

        [Test]
        public void GenerarPrecioDeHabitacionPorFechaYUsuario_TarifaFriday()
        {
            ///si recibimos fecha, aplicar tarifas de fin de semana
            //Arrange
            var usuario = new Usuario { Id = 676, EsUsuarioPremium = false };
            var habitacion = new Habitacion { Id = 4545, TarifaBase = 500m };
            var dia = new DateTime(2019,03, 29);
            var servicio = new ServiciosReservacionLocales();
            //Act
            var resultado = servicio.GenerarPrecioDeHabitacionPorFechaYUsuario(
                habitacion, dia, usuario);
            //Assert
            Assert.AreEqual(525m, resultado.Precio);
        }

        [Test]
        public void GenerarPrecioDeHabitacionPorFechaYUsuario_TarifaSatSunDay()
        {
            //si recibimos fecha, aplicar tarifas de fin de semana
            //Arrange
            var usuario = new Usuario { Id = 676, EsUsuarioPremium = false };
            var habitacion = new Habitacion { Id = 4545, TarifaBase = 500m };
            var dia = new DateTime(2019, 03, 30);
            var servicio = new ServiciosReservacionLocales();
            //Act
            var resultado = servicio.GenerarPrecioDeHabitacionPorFechaYUsuario(
                habitacion, dia, usuario);
            //Assert
            Assert.AreEqual(575m, resultado.Precio);
        }

        [Test]
        public void GenerarPrecioDeHabitacionPorFechaYUsuario_TarifaSatSunDay_Premium()
        {
            //si recibimos un usuario y es un usuario premium, aplicar descuento de 10%
            //si recibimos fecha, aplicar tarifas de fin de semana
            //Arrange
            var usuario = new Usuario { Id = 676, EsUsuarioPremium = true };
            var habitacion = new Habitacion { Id = 4545, TarifaBase = 500m };
            var dia = new DateTime(2019, 03, 30);
            var servicio = new ServiciosReservacionLocales();
            //Act
            var resultado = servicio.GenerarPrecioDeHabitacionPorFechaYUsuario(
                habitacion, dia, usuario);
            //Assert
            Assert.AreEqual(525m, resultado.Precio);
        }
        #endregion

        #region GenerarPrecioDeHabitacionPorFechaYUsuario
        [Test]
        public void GenerarPrecioDeHabitacionPorFechaYUsuario_valida_precios()
        {
            // Regresa el precio de la ha bitacion por dia para cada dia del rango de fechas
            var habitacion = new Habitacion() {Tipo = TipoHbitacion.Sencilla,
                                            Ocupacion = EstadoOcupacion.Vacio,
                                            TarifaBase = 500m};//usuario premium

            var usuario = new Usuario { Id = 56, EsUsuarioPremium = true };
            var ListaResult = new List<CostoHabitacionPorDia>();
            var servicios = new ServiciosReservacionLocales();//servicio

            var Finicio = new DateTime(2019,03,25);
            var Ffin = new DateTime(2019,03,31);
            //Act
            ListaResult = servicios.GenerarPrecioDeHabitacionPorFechaYUsuario(
                habitacion,Finicio,Ffin,usuario);
            decimal total = ListaResult.Sum(item => item.Precio);

            Assert.AreEqual(3325m, total);


            //Assert
            //foreach (CostoHabitacionPorDia CostoXd in ListaResult)
            //{
            //    Assert.AreEqual(servicios.GenerarPrecioDeHabitacionPorFechaYUsuario(habitacion, CostoXd.Fecha, usuario).Precio, CostoXd.Precio);
            //}


        }
        #endregion
    }
}
