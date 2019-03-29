using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reservaciones.Entidades;
using Reservaciones.Servicios;
using NUnit.Framework;
using Moq;
using Reservaciones.Servicios.BaseDeDatos;

namespace Reservaciones.UnitTests
{
    [TestFixture]
    public class ServiciosReservacionDependientesTests
    {
        #region Test_BuscarHabitaciones
        [Test]
        public void BuscarHabitaciones_FechaEntradaMayorFechaSalida_Error()
        {
            //Arrange
            var servicio = new ServiciosReservacionDependientes();
            var usuario = new Usuario();
            DateTime FechaIn = DateTime.Now;
            DateTime FechaOut = DateTime.Now.AddDays(-1);//Fecha de salida menor a entrada
            List<Habitacion> Habitaciones = new List<Habitacion>
            { new Habitacion {Id=1, Ocupacion = EstadoOcupacion.Vacio },
              new Habitacion {Id=2, Ocupacion = EstadoOcupacion.Vacio },};
            List<Reservacion> Reservaciones = new List<Reservacion> {
                new Reservacion{Id = 1, Usuario = usuario } };
                //Mock servicios 
            var ObjetoFalsoHabitaciones = new Mock<IRepositorioHabitaciones>();
            ObjetoFalsoHabitaciones
                .Setup(m => m.SeleccionarhabitacionesDisponibles(FechaIn, FechaOut))
                .Returns(Habitaciones);
            var ObjetoFalsoReservaciones = new Mock<IRepositorioReservaciones>();
            ObjetoFalsoReservaciones
                .Setup(m => m.BuscarReservacionesPorUsuario(usuario))
                .Returns(Reservaciones);
            //Act
            void Metodo() => servicio.BuscarHabitaciones(FechaIn,FechaOut,usuario);
            //Assert ArgumentException
            Assert.Throws<ArgumentException>(Metodo);
        }

        [Test]
        public void BuscarHabitaciones_HabitacionesNoReservadasLista()
        {
            //agregar habitaciones, que el usuario no haya reservado en esas fechas, a los resultados
            //Arrange
            
            var usuario = new Usuario() { Id = 555};
            var HabitaReserv = new Habitacion { Id = 11, Ocupacion = EstadoOcupacion.Ocupado };
            var CostHabitaReserv = new CostoHabitacionPorDia(HabitaReserv, 100m, DateTime.Now);

            DateTime FechaIn = DateTime.Now;
            DateTime FechaOut = DateTime.Now.AddDays(4);//Fecha de salida menor a entrada

            List<Habitacion> Habitaciones = new List<Habitacion>
            { new Habitacion {Id=1, Ocupacion = EstadoOcupacion.Ocupado},
              new Habitacion {Id=2, Ocupacion = EstadoOcupacion.Vacio },
              new Habitacion {Id=3, Ocupacion = EstadoOcupacion.Vacio },
              new Habitacion {Id=4, Ocupacion = EstadoOcupacion.Vacio }};

            //Habitaciones.Add(new Habitacion { });

            List<Reservacion> Reservaciones = new List<Reservacion> {
                new Reservacion{Id = 11, Usuario = usuario,
                    HabitacionesReservadas = new List<CostoHabitacionPorDia> {CostHabitaReserv } }
                 };
            //Mock servicios 
            var ObjetoFalsoHabitaciones = new Mock<IRepositorioHabitaciones>();
            ObjetoFalsoHabitaciones
                .Setup(m => m.SeleccionarhabitacionesDisponibles(FechaIn, FechaOut))
                .Returns(Habitaciones);
            var ObjetoFalsoReservaciones = new Mock<IRepositorioReservaciones>();
            ObjetoFalsoReservaciones
                .Setup(m => m.BuscarReservacionesPorUsuario(usuario))
                .Returns(Reservaciones);

            var servicio = new ServiciosReservacionDependientes(ObjetoFalsoHabitaciones.Object,
                                                                ObjetoFalsoReservaciones.Object);
            //Act
            List<CostoHabitacionPorDia> result = servicio.BuscarHabitaciones(FechaIn, FechaOut, usuario);
            //Assert ArgumentException
            Assert.IsFalse(result.Contains(CostHabitaReserv));
        }
        #endregion
    }
}
