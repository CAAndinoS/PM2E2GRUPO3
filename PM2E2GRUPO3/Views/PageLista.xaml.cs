using Android.Icu.Text;
using Android.Media;
using PM2E2GRUPO3.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageLista : ContentPage
    {
        public PageLista()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            List<Models.Sitios> sitio = new List<Models.Sitios>();
            sitio = await Controller.SitioController.GetSitios();
            ListaEmpleados.ItemsSource = sitio;
        }

        private async void ListaEmpleados_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
            String sexResult = await DisplayActionSheet("Seleccione una opción ", "Cancelar", null, "Actualizar", "Mapa", "Eliminar", "Reproducir");
            var d = e.SelectedItem as Models.Sitios;

            if (sexResult == "Reproducir")
            {
                MediaPlayer mediaPlayer = new MediaPlayer();
                mediaPlayer.SetDataSource(new Controller.Reproductor(Convert.FromBase64String(d.audio)));
                //mediaPlayer.Reset();
                mediaPlayer.Prepare();
                mediaPlayer.Start();
            }

            if (sexResult == "Eliminar")
            {
                // Crear el objeto Sitios con la información del sitio a eliminar
                Models.Sitios sitio = new Models.Sitios
                {
                    id = d.id
                };

                // Llamar al método DeleteSitio para eliminar el sitio
                Models.Msg deleteMsg = await SitioController.DeleteSit(sitio.id);

                // Verificar la respuesta y mostrar una notificación
                if (deleteMsg != null && deleteMsg.Success)
                {
                    await DisplayAlert("¡Notificación!", "Datos Eliminados", "OK");

                }
                else
                {
                    await DisplayAlert("¡Error!", "No se pudo eliminar el sitio", "OK");
                }
            }

            if (sexResult == "Actualizar")
            {

                var newpage = new PageEditar(d);
                Debug.WriteLine(d.foto);
                await Navigation.PushAsync(newpage);
            }

            if (sexResult == "Mapa")
            {
                var location = new Location(Double.Parse(d.latitud), Double.Parse(d.longitud));

                try
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                    var locationResult = await Geolocation.GetLocationAsync(request);

                    if (locationResult != null)
                    {
                        var currentLocation = new Location(locationResult.Latitude, locationResult.Longitude);

                        var distance = Location.CalculateDistance(location, currentLocation, DistanceUnits.Kilometers);
                        if (distance <= 0.1) // 100 metros en kilómetros es 0.1
                        {
                            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
                            await Map.OpenAsync(location, options);
                        }
                    }
                    else
                    {
                        // No se pudo obtener la ubicación actual
                        await DisplayAlert("Error", "No se pudo obtener la ubicación actual del GPS", "OK");
                    }
                }
                catch (FeatureNotSupportedException)
                {
                    // La funcionalidad de ubicación no es compatible en el dispositivo
                    await DisplayAlert("Error", "La funcionalidad de ubicación no es compatible en este dispositivo", "OK");
                }
                catch (PermissionException)
                {
                    // No se han otorgado los permisos necesarios para acceder a la ubicación
                    await DisplayAlert("Error", "No se han otorgado los permisos necesarios para acceder a la ubicación", "OK");
                }
                catch (Exception)
                {
                    // Ocurrió un error al obtener la ubicación actual
                    await DisplayAlert("Error", "Ocurrió un error al obtener la ubicación actual", "OK");
                }
            }

            /* audio*/
            String b64 = d.audio;
            
        }
    }
}