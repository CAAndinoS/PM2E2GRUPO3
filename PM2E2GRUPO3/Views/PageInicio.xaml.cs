using Android.Graphics;
using Newtonsoft.Json.Linq;
using Plugin.AudioRecorder;
using Plugin.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public partial class PageInicio : ContentPage
    {
        string ruta = "", StringBase64Foto = "", StringBase64Audio = "", StringBase64Video = ""; //ruta de la imagen
        int aud = 0; // validaciones
        AudioRecorderService recorder = new AudioRecorderService();// grabadora
        private AudioPlayer player = new AudioPlayer();
        Plugin.Media.Abstractions.MediaFile photo = null;
        public PageInicio()
        {
            InitializeComponent();
            InizializatePlugins();
        }

        private String traeImagenToBase64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();

                    String Base64 = Convert.ToBase64String(fotobyte);

                    return Base64;
                }
            }

            return null;

        }

        private byte[] obtener_imagen_escalada(byte[] imagen, int compresion)
        {
            Bitmap original = BitmapFactory.DecodeByteArray(imagen, 0, imagen.Length);
            using (MemoryStream memory = new MemoryStream())
            {
                original.Compress(Bitmap.CompressFormat.Jpeg, compresion, memory);
                return memory.ToArray();
            }

        }

        private async void InizializatePlugins()
        {

            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null)
                {
                    lblLatitud.Text = location.Latitude.ToString();
                    lblLongitud.Text = location.Longitude.ToString();
                }
                else
                {
                    await DisplayAlert("¡ALERTA!", "ACTIVAR GPS", "OK");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }



        }

        bool verificarInternet()
        {

            var current = Connectivity.NetworkAccess;

            return current != NetworkAccess.Internet;

        }

        private async void guardar()
        {

            if (String.IsNullOrWhiteSpace(txtDescripcion.Text))
            {

                await DisplayAlert("Error", "No completó todos los campos", "OK");
            }
            else if (verificarInternet())
            {
                await DisplayAlert("¡AVISO!", "NO TIENE ACCESO A INTERNET, POR FAVOR ACTIVARLO", "OK");
            }
            else
            {
                var sit = new Models.Sitios
                {
                    descripcion = txtDescripcion.Text,
                    longitud = lblLongitud.Text,
                    latitud = lblLatitud.Text,
                    audio = TraerAudioToBase64(),
                    foto = traeImagenToBase64()
                };
                Models.Msg resultado = await Controller.SitioController.CreateSit(sit);
                if (resultado != null)
                {
                    await DisplayAlert("aviso", resultado.message.ToString(), "OK");
                }
            }

        }

        private async void btnFoto_Clicked(object sender, EventArgs e)
        {



            if (string.IsNullOrEmpty(txtDescripcion.Text))
            {
                await DisplayAlert("Alerta", "Antes de Tomar el Video Debe Llenar los Campos Vacios", "Ok");
            }
            else
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "MYAPP",
                    Name = "Foto.jpg",
                    SaveToAlbum = true
                });
                if (photo != null)
                {
                    Foto.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                }
            }

        }

        private void reset()
        {
            txtDescripcion.Text = "";
            ruta = "";
            
            aud = 0;
            lblAudio.Text = "Estatus Audio";
            lblAudio.TextColor = System.Drawing.Color.Blue;
            recorder = new AudioRecorderService();
        }

        private async void btnGrabar_Clicked(object sender, EventArgs e)
        {
            // Verificar si los permisos están otorgados
            var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                // Si los permisos no están otorgados, solicitarlos
                status = await Permissions.RequestAsync<Permissions.Microphone>();
            }

            // Verificar el resultado de la solicitud de permisos
            if (status == PermissionStatus.Granted)
            {
                // Los permisos están otorgados, continuar con la grabación
                try
                {
                    if (!recorder.IsRecording)
                    {
                        lblAudio.Text = "Grabando";
                        await recorder.StartRecording();
                    }
                }
                catch (Exception)
                {
                    lblAudio.Text = "Estatus Audio";
                    await DisplayAlert("¡ALERTA!", "ACTIVAR PERMISOS DE MICRÓFONO", "OK");
                }
            }
            else
            {
                // Los permisos no están otorgados, mostrar un mensaje de alerta o realizar alguna otra acción
                lblAudio.Text = "Estatus Audio";
                await DisplayAlert("¡ALERTA!", "ACTIVAR PERMISOS DE MICRÓFONO", "OK");
            }
        }

        private async void btnDetener_Clicked(object sender, EventArgs e)
        {
            if (recorder.IsRecording)
            {
                lblAudio.Text = "Audio Detenido";
                await recorder.StopRecording();
                aud = 1;
            }
        }

        private string TraerAudioToBase64()
        {
            byte[] audioBytes = null;

            using (var stream = new MemoryStream())
            {
                recorder.GetAudioFileStream().CopyTo(stream);
                audioBytes = stream.ToArray();
            }

            return Convert.ToBase64String(audioBytes);
        }

        private void btnReproducir_Clicked(object sender, EventArgs e)
        {
            var filePath = recorder.GetAudioFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                lblAudio.Text = "Reproduciendo";
                player.Play(filePath);
                lblAudio.Text = "Sin acción";
            }
        }

        private void btnGuardar_Clicked(object sender, EventArgs e)
        {
            guardar();
        }

        private async void btnListar_Clicked(object sender, EventArgs e)
        {

            if (verificarInternet())
            {
                await DisplayAlert("¡AVISO!", "NO TIENE ACCESO A INTERNET, POR FAVOR ACTIVARLO", "OK");
                return;
            }
            var am = new PageLista();
            await Navigation.PushAsync(am);
        }
    }
}