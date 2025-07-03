using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using System.Text;
using InTheHand.Net;

namespace ESP32Parasito;

public partial class MainPage : ContentPage
{
    private BluetoothClient _bluetoothClient;
    private Stream _stream;

    private readonly string esp32Mac = "D4:8C:49:E3:AF:CE";

    public MainPage()
    {
        InitializeComponent();

        btnConectar.Clicked += OnConnectClicked;
        btnForward.Clicked += (s, e) => SendCommand('w');
        btnBackward.Clicked += (s, e) => SendCommand('s');
        btnLeft.Clicked += (s, e) => SendCommand('a');
        btnRight.Clicked += (s, e) => SendCommand('d');
        btnStop.Clicked += (s, e) => SendCommand('q');
    }

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            txtEstatus.Text = "Conectando...";
            btnConectar.IsEnabled = false;
            btnConectar.Text = "Conectando...";

            await Task.Run(() =>
            {
                var address = BluetoothAddress.Parse(esp32Mac);
                _bluetoothClient = new BluetoothClient();
                _bluetoothClient.Connect(address, BluetoothService.SerialPort);
                _stream = _bluetoothClient.GetStream();
            });

            txtEstatus.Text = "Conectado a ESP32";
            btnConectar.Text = "Conectado";
            btnConectar.BackgroundColor = Colors.MediumSeaGreen;
            btnConectar.IsEnabled = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo conectar al ESP32:\n{ex.Message}", "OK");
            txtEstatus.Text = "Estado: Desconectado";
            btnConectar.Text = "Conectar";
            btnConectar.BackgroundColor = Color.FromArgb("#FF9800");
            btnConectar.IsEnabled = true;
        }
    }

    private async void SendCommand(char command)
    {
        try
        {
            if (_stream == null || !_stream.CanWrite)
            {
                await DisplayAlert("Error", "No hay conexión activa. Conectate primero.", "OK");
                return;
            }

            byte[] buffer = Encoding.ASCII.GetBytes(new[] { command });
            await _stream.WriteAsync(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo enviar el comando:\n{ex.Message}", "OK");
        }
    }
}
