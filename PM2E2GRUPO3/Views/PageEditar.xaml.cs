using Android.Graphics;
using Newtonsoft.Json.Linq;
using Plugin.AudioRecorder;
using Plugin.Media;
using PM2E2GRUPO3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Resource;

namespace PM2E2GRUPO3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PageEditar : ContentPage
    {
        string ruta = "";
        AudioRecorderService recorder = new AudioRecorderService();// grabadora
        private AudioPlayer player = new AudioPlayer();
        int aud = 0; // validaciones
        Sitios sitio;
        public PageEditar(Sitios s)
        {
            InitializeComponent();
            sitio = s;
            Foto.Source = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(sitio.foto)));
            lblLatitud.Text = sitio.latitud;
            lblLongitud.Text = sitio.longitud;
            txtDescripcion.Text = sitio.descripcion;
        }
        Plugin.Media.Abstractions.MediaFile photo = null;

        private System.String traeImagenToBase64()
        {
            if (photo != null)
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Stream stream = photo.GetStream();
                    stream.CopyTo(memory);
                    byte[] fotobyte = memory.ToArray();

                    System.String Base64 = Convert.ToBase64String(fotobyte);

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


        bool verificarInternet()
        {

            var current = Connectivity.NetworkAccess;

            return current != NetworkAccess.Internet;

        }


        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            if (System.String.IsNullOrWhiteSpace(txtDescripcion.Text))
            {

                await DisplayAlert("Error", "No se relleno todos los campos", "OK");
            }
            else if (verificarInternet())
            {
                await DisplayAlert("¡AVISO!", "NO TIENE ACCESO A INTERNET, POR FAVOR ACTIVARLO", "OK");
                return;
            }
            else
            {

                var sit = new Models.Sitios
                {
                    id = sitio.id,
                    descripcion = txtDescripcion.Text,
                    longitud = lblLongitud.Text,
                    latitud = lblLatitud.Text,
                    audio = string.IsNullOrEmpty(sitio.audio) ? TraerAudioToBase64() : sitio.audio,
                    foto = string.IsNullOrEmpty(sitio.foto) ? traeImagenToBase64() : sitio.foto
                };

                Models.Msg resultado = await Controller.SitioController.UpdateSit( sit);
                if (resultado != null)
                {
                    await DisplayAlert("aviso", resultado.message.ToString(), "OK");
                }

            }
        }



        private void btnReproducirNuevo_Clicked(object sender, EventArgs e)
        {
            var filePath = recorder.GetAudioFilePath();
            if (!string.IsNullOrEmpty(filePath))
            {
                lblAudio.Text = "Reproduciendo";
                player.Play(filePath);
                lblAudio.Text = "Sin acción";
            }
        }

        private void reset()
        {
            txtDescripcion.Text = "";
            ruta = "";
            imgFoto.Source = "paisajes.gif";
            aud = 0;
            lblAudio.Text = "Estatus Audio";
            lblAudio.TextColor = System.Drawing.Color.Blue;
            recorder = new AudioRecorderService();
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
    }
}