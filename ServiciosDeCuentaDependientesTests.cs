using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Banco.Servicios;
using Banco.Entidades;
using Banco.Servicios.ServiciosDeTerceros;

namespace Banco.UnitTests
{
    [TestFixture]
    public class ServiciosDeCuentaDependientesTests
    {
        #region Login

        [Test]
        public void Login_UsuarioNoExiste_RegresaFalso()
        {
            //Arrange
            var nombreusuario = "Jhon1";
            var pass = "1234";
            var objetoFalso = new Mock<IRepositorioUsuarios>();
            objetoFalso.Setup(m => m.SeleccionarUsuario(nombreusuario))
                .Returns((Usuario)null);

            var servicio = new ServiciosDeCuentaDependientes(
                objetoFalso.Object,null,null,null,null);
            //Act
            var resultado = servicio.Login(nombreusuario, pass);
            //Asert
            Assert.IsFalse(resultado);
        }

        [Test]
        public void Login_UsuarioInactivo_RegresaFalso()
        {
            //Arrange
            var nombreusuario = "Jhon1";
            var pass = "1234";

            var ServicioFalso = new Mock<IRepositorioUsuarios>();
            ServicioFalso.Setup(m => m.SeleccionarUsuario(nombreusuario))
                .Returns(new Usuario() { EstaActivo = false });
            var servicioAprobar = new ServiciosDeCuentaDependientes(
                ServicioFalso.Object,null,null,null,null);
            
            //Act

            var resultado = servicioAprobar.Login(nombreusuario, pass);
            //Asert

            Assert.IsFalse(resultado);

        }

        [Test]
        public void Login_UsuarioActivoYPasswordIncorrecto_RegresaFalso()
        {
            //Arrange
            var nombreusuario = "Jhon1";
            var pass = "1234";

            var ServicioFalso = new Mock<IRepositorioUsuarios>();
            ServicioFalso.Setup(m => m.SeleccionarUsuario(nombreusuario))
                .Returns(new Usuario() { EstaActivo = true, Password = "abcd" });
            var servicioAprobar = new ServiciosDeCuentaDependientes(
                ServicioFalso.Object,null,null,null,null);

            //Act

            var resultado = servicioAprobar.Login(nombreusuario, pass);
            //Asert

            Assert.IsFalse(resultado);
        }

        [Test]
        public void Login_UsuarioActivoYPasswordCorrecto_RegresaVerdadero()
        {
            //Arrange
            var nombreusuario = "Jhon1";
            var pass = "1234";

            var ServicioFalso = new Mock<IRepositorioUsuarios>();
            ServicioFalso.Setup(m => m.SeleccionarUsuario(nombreusuario))
                .Returns(new Usuario() { EstaActivo = true, Password = "1234" });
            var servicioAprobar = new ServiciosDeCuentaDependientes(
                ServicioFalso.Object,null,null,null,null);

            //Act

            var resultado = servicioAprobar.Login(nombreusuario, pass);
            //Asert

            Assert.IsTrue(resultado);
        }

        #endregion Login

        #region SolicitarTarjeta
        [Test]
        public void SolicitarTarjeta_UsuarioyatieneTarjetasIgual_regresaFalso()
        {
            //verificar si el usuario ya tiene una tarjeta de ese tipo
            //Arrange
            var usuario = new Usuario(){
                Cuentas = new ICuentaBancaria[] { new TarjetaClasica() }
            };
            var tarjetaSolicitada = new TarjetaClasica();

            var servicio = new ServiciosDeCuentaDependientes();

            //Act
            var resultado = servicio.SolicitarTarjeta(usuario, tarjetaSolicitada);

            //Assert
            Assert.IsFalse(resultado);

        }

        [Test]
        public void SolicitarTarjeta_UsuarioExcedeMaximoTarjetas_regresaFalso()
        {
            //consultar # maximo de tarjetas
            //Arrange
            var usuario = new Usuario() { Cuentas = new ICuentaBancaria[] {
                new TarjetaClasica(),
                new TarjetaOro()
                },
                RFC = "rfceseste"
            };

            var tarjeta = new TarjetaPlatino();

            var MockRepoUsuarios = new Mock<IRepositorioUsuarios>();
            var MockConfiguraciones = new Mock<IRepositorioConfiguraciones>();
            var MockServicioBuro = new Mock<IServicioExternoBuro>();

            var servicio = new ServiciosDeCuentaDependientes(MockRepoUsuarios.Object,
                                                            MockConfiguraciones.Object,
                                                            MockServicioBuro.Object,
                                                            null,null);
            MockConfiguraciones
                .Setup(m => m.SeleccionarMaximoDeTarjetasPorUsuario())
                .Returns(1);
            MockServicioBuro
                .Setup(m => m.ConsultarBuro(usuario.RFC))
                .Returns(90m);


            //Act
            var resultado = servicio.SolicitarTarjeta(usuario, tarjeta);

            //Assert
            Assert.IsFalse(resultado);

        }

        [Test]
        public void SolicitarTarjeta_UsuarioRFC_Calificacion_BajaParaTarjetaClasica_RegresaFalso()
        {
            //consultar buro de credito
            //Arrange
            var usuario = new Usuario()
            {
                Cuentas = new ICuentaBancaria[] {
                new CuentaDeAhorro()
                },
                RFC = "rfceseste"
            };

            var tarjeta = new TarjetaClasica();

            var MockRepoUsuarios = new Mock<IRepositorioUsuarios>();
            var MockConfiguraciones = new Mock<IRepositorioConfiguraciones>();
            var MockServicioBuro = new Mock<IServicioExternoBuro>();

            var servicio = new ServiciosDeCuentaDependientes(MockRepoUsuarios.Object,
                                                            MockConfiguraciones.Object,
                                                            MockServicioBuro.Object,
                                                            null,null);
            MockConfiguraciones
                .Setup(m => m.SeleccionarMaximoDeTarjetasPorUsuario())
                .Returns(1);
            MockServicioBuro
                .Setup(m => m.ConsultarBuro(usuario.RFC))
                .Returns(25m);


            //Act
            var resultado = servicio.SolicitarTarjeta(usuario, tarjeta);

            //Assert
            Assert.IsFalse(resultado);

        }

        [Test]
        public void SolicitarTarjeta_UsuarioRFC_Calificacion_BajaParaTarjetaOro_RegresaFalso()
        {
            //consultar buro de credito
            //Arrange
            var usuario = new Usuario()
            {
                Cuentas = new ICuentaBancaria[] {
                new CuentaDeAhorro()
                },
                RFC = "rfceseste"
            };

            var tarjeta = new TarjetaOro();

            var MockRepoUsuarios = new Mock<IRepositorioUsuarios>();
            var MockConfiguraciones = new Mock<IRepositorioConfiguraciones>();
            var MockServicioBuro = new Mock<IServicioExternoBuro>();

            var servicio = new ServiciosDeCuentaDependientes(MockRepoUsuarios.Object,
                                                            MockConfiguraciones.Object,
                                                            MockServicioBuro.Object,
                                                            null,null);
            MockConfiguraciones
                .Setup(m => m.SeleccionarMaximoDeTarjetasPorUsuario())
                .Returns(1);
            MockServicioBuro
                .Setup(m => m.ConsultarBuro(usuario.RFC))
                .Returns(60m);


            //Act
            var resultado = servicio.SolicitarTarjeta(usuario, tarjeta);

            //Assert
            Assert.IsFalse(resultado);

        }

        [Test]
        public void SolicitarTarjeta_UsuarioRFC_Calificacion_BajaParaTarjetaPlatino_RegresaFalso()
        {
            //consultar buro de credito
            //Arrange
            var usuario = new Usuario()
            {
                Cuentas = new ICuentaBancaria[] {
                new CuentaDeAhorro()
                },
                RFC = "rfceseste"
            };

            var tarjeta = new TarjetaPlatino();

            var MockRepoUsuarios = new Mock<IRepositorioUsuarios>();
            var MockConfiguraciones = new Mock<IRepositorioConfiguraciones>();
            var MockServicioBuro = new Mock<IServicioExternoBuro>();

            var servicio = new ServiciosDeCuentaDependientes(MockRepoUsuarios.Object,
                                                            MockConfiguraciones.Object,
                                                            MockServicioBuro.Object,
                                                            null,null);
            MockConfiguraciones
                .Setup(m => m.SeleccionarMaximoDeTarjetasPorUsuario())
                .Returns(1);
            MockServicioBuro
                .Setup(m => m.ConsultarBuro(usuario.RFC))
                .Returns(80m);


            //Act
            var resultado = servicio.SolicitarTarjeta(usuario, tarjeta);

            //Assert
            Assert.IsFalse(resultado);

        }

        #endregion SolicitarTarjeta

        #region TransferirATerceros
        [Test]
        public void TransferirATerceros_CuentaDestinoEsLocal_regresaError()
        {
            //Arrange
            var cuentaOrigen = new CuentaDeAhorro();
            var CuentaDest = new CuentaDeAhorro();//Cuenta local
            var Coin = new Moneda(1000m, Divisa.MXN);
            var servicio = new ServiciosDeCuentaDependientes();
            //Act
            void  MetodoAProbar() => servicio.TransferirATerceros(cuentaOrigen,CuentaDest,Coin);
            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAProbar);
        }

        [Test]
        public void TransferirATerceros_DivisasDiferentes_regresaError()
        {
            //Arrange
            var cuentaOrigen = new CuentaDeAhorro() {
                Balance = new Moneda(100m,Divisa.MXN)};
            var CuentaDest = new CuentaDeAhorroExterna() {
                Balance = new Moneda(100m, Divisa.USD)};//Divisa USD
            var Coin = new Moneda(1000m, Divisa.MXN);
            var servicio = new ServiciosDeCuentaDependientes();
            //Act
            void MetodoAProbar() => servicio.TransferirATerceros(cuentaOrigen, CuentaDest, Coin);
            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAProbar);

        }

        [Test]
        public void TransferirATerceros_UsuariosDiferentes_regresaError()
        {
            //Arrange
            var user1 = new Usuario() {Id = 555 };
            var user2 = new Usuario() { Id = 556 };//Diferente usuario
            var cuentaOrigen = new CuentaDeAhorro()
            {
                Titular = user1,
                Balance = new Moneda(100m, Divisa.MXN)
            };
            var CuentaDest = new CuentaDeAhorroExterna()
            {
                Titular = user2,
                Balance = new Moneda(100m, Divisa.MXN),
                CLABE = "SCLABES",
                Banco = "BBVA"
            };

            var Coin = new Moneda(1000m, Divisa.MXN);
            var MockServicioSPEI = new Mock<IServicioExternoSPEI>();

            var servicio = new ServiciosDeCuentaDependientes(null, null, null,
                                                            MockServicioSPEI.Object,null);
            MockServicioSPEI
                .Setup(m => m.EnviarSpei(CuentaDest.Banco, CuentaDest.CLABE, Coin.Cantidad))
                .Returns(true);

            //Act
            void MetodoAProbar() => servicio.TransferirATerceros(cuentaOrigen, CuentaDest, Coin);
            //Assert
            Assert.Throws<InvalidOperationException>(MetodoAProbar);

        }

        [Test]
        public void TransferirATerceros_ServicioSPEI_ReturnFALSE_ERROR()
        {
            //Arrange
            var user1 = new Usuario() { Id = 555 };
            var user2 = new Usuario() { Id = 555 };
            var cuentaOrigen = new CuentaDeAhorro()
            {
                Titular = user1,
                Balance = new Moneda(100m, Divisa.MXN)
            };
            var CuentaDest = new CuentaDeAhorroExterna()
            {
                Titular = user2,
                Balance = new Moneda(100m, Divisa.MXN),
                CLABE = "SCLABES",
                Banco = "BBVA"
            };

            var Coin = new Moneda(1000m, Divisa.MXN);
            var MockServicioSPEI = new Mock<IServicioExternoSPEI>();

            var servicio = new ServiciosDeCuentaDependientes(null, null, null,
                                                            MockServicioSPEI.Object,null);
            MockServicioSPEI
                .Setup(m => m.EnviarSpei(CuentaDest.Banco, CuentaDest.CLABE, Coin.Cantidad))
                .Returns(false);//No se realizo el movimiento en SPEI

            //Act
            void MetodoAProbar() => servicio.TransferirATerceros(cuentaOrigen, CuentaDest, Coin);
            //Assert
            Assert.Throws<ApplicationException>(MetodoAProbar);

        }

        [Test]
        public void TransferirATerceros_ServicioSPEI_HacerCargoComision()
        {
            //Arrange
            var user1 = new Usuario() { Id = 555 };
            var user2 = new Usuario() { Id = 555 };//Diferente usuario
            var cuentaOrigen = new CuentaDeAhorro()
            {
                Titular = user1,
                Balance = new Moneda(1000m, Divisa.MXN)
            };
            var CuentaDest = new CuentaDeAhorroExterna()
            {
                Titular = user2,
                Balance = new Moneda(100m, Divisa.MXN),
                CLABE = "SCLABES",
                Banco = "BBVA"
            };//Cuenta local

            var Coin = new Moneda(150m, Divisa.MXN);//Envia 150 MXN

            var MockServicioSPEI = new Mock<IServicioExternoSPEI>();

            var servicio = new ServiciosDeCuentaDependientes(null, null, null,
                                                            MockServicioSPEI.Object,null);
            MockServicioSPEI
                .Setup(m => m.EnviarSpei(CuentaDest.Banco, CuentaDest.CLABE, Coin.Cantidad))
                .Returns(true);//No se realizo el movimiento en SPEI

            //Act
            servicio.TransferirATerceros(cuentaOrigen, CuentaDest, Coin);
            //Assert
            Assert.AreEqual(new Moneda(848.5m, Divisa.MXN),cuentaOrigen.Balance);
        }

        #endregion TransferirATerceros

        #region ConvertirMoneda
        [Test]
        public void ConvertirMoneda_ValidarConversionaEUR()
        {
            //Arrange
            var Coin = new Moneda(1000m, Divisa.MXN);
            var MockServicioExternoTipoCambio = new Mock<IServicioExternoTipoDeCambio>();
            MockServicioExternoTipoCambio
                .Setup(m => m.TipoDeCambio(Coin.Divisa, Divisa.EUR))
                .Returns(0.046m);
            var servicio = new ServiciosDeCuentaDependientes(null,
                null, null, null, MockServicioExternoTipoCambio.Object);

            //Act
            var result = servicio.ConvertirMoneda(Coin, Divisa.EUR);

            //Assert
            Assert.AreEqual(new Moneda(46m, Divisa.EUR), result);
        }

        #endregion ConvertirMoneda

        #region ConvertirPesosADolares
        [Test]
        public void ConvertirPesosADolares()
        {
            //Arrange
            var MockServicioExternoTipoCambio = new Mock<IServicioExternoTipoDeCambio>();
            MockServicioExternoTipoCambio
                .Setup(m => m.TipoDeCambio(Divisa.MXN, Divisa.USD))
                .Returns(0.052m);
            var servicio = new ServiciosDeCuentaDependientes(null,
                null, null, null, MockServicioExternoTipoCambio.Object);

            //Act
            var result = servicio.ConvertirPesosADolares(1000m);
            //Assert
            Assert.AreEqual(new Moneda(52m, Divisa.USD), result);
        }

        #endregion ConvertirPesosADolares

        #region ConvertirPesosAEuros
        [Test]
        public void ConvertirPesosAEuros()
        {
            //Arrange
            var MockServicioExternoTipoCambio = new Mock<IServicioExternoTipoDeCambio>();
            MockServicioExternoTipoCambio
                .Setup(m => m.TipoDeCambio(Divisa.MXN, Divisa.EUR))
                .Returns(0.046m);
            var servicio = new ServiciosDeCuentaDependientes(null,
                null, null, null, MockServicioExternoTipoCambio.Object);

            //Act
            var result = servicio.ConvertirPesosAEuros(1000m);
            //Assert
            Assert.AreEqual(new Moneda(46m, Divisa.EUR), result);
        }

        #endregion ConvertirPesosAEuros
    }
}
