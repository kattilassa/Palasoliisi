using Godot;
using System;
using System.Collections.Generic;
[Tool]
public partial class VolumeSetting : HBoxContainer
{
    private static Dictionary<int, double> _volumePercentageByBusIndex = new Dictionary<int, double>();
    private const string ConfigPath = "user://audio_settings.cfg";
    private string _busName = "<Bus Name>";
    private Label _busNameLabel;
    private HSlider _volumeSlider;
    private Label _volumePercentageLabel;
    private AudioStreamPlayer _audioStreamPlayer;
    private Timer _timer = new Timer();

    [Export] public string BusName
    {
        get => _busName;
        set
        {
            _busName = value;
            UpdateBusNameLabel();
        }
    }

    [Export] public int Busindex { get; set; }

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            foreach (var child in GetChildren())
            {
                if (child is AudioStreamPlayer audioStreamPlayerChild)
                {
                    _audioStreamPlayer = audioStreamPlayerChild;
                    AddChild(_timer);
                    _timer.WaitTime = 1.0f;
                    _timer.OneShot = true;
                }
            }
        }

        _busNameLabel = GetNode<Label>("BusNameLabel");
        UpdateBusNameLabel();

        _volumeSlider = GetNode<HSlider>("VolumeHSlider");
        _volumeSlider.ValueChanged += _volumeSlider_ValueChanged;

        _volumePercentageLabel = GetNode<Label>("VolumePercentageLabel");

        LoadVolumeSettings();
    }

    private void _volumeSlider_ValueChanged(double value)
    {
        UpdateVolumePercentageLabel();
        _volumePercentageByBusIndex[Busindex] = value;

        if (value == 0)
        {
            AudioServer.SetBusMute(Busindex, true);
        }
        else
        {
            if (AudioServer.IsBusMute(Busindex))
                AudioServer.SetBusMute(Busindex, false);

            float decibels = ConvertPercentageToDecibels(value);
            AudioServer.SetBusVolumeDb(Busindex, decibels);
        }

        if (_audioStreamPlayer != null && _timer.TimeLeft == 0)
        {
            _audioStreamPlayer.Play();
            _timer.Start();
        }

        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        ConfigFile config = new ConfigFile();

        foreach (var entry in _volumePercentageByBusIndex)
        {
            config.SetValue("Audio", $"Bus_{entry.Key}_volume", entry.Value);
        }

        config.Save(ConfigPath);
    }

    private void LoadVolumeSettings()
    {
        ConfigFile config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok) return;

        if (config.HasSection("Audio"))
        {
            double defaultVolume = 50.0;
            double volume = (double)config.GetValue("Audio", $"Bus_{Busindex}_volume", defaultVolume);
            _volumeSlider.Value = volume;
        }
        else
        {
            _volumeSlider.Value = 50.0;
        }
        UpdateVolumePercentageLabel();
    }

    private void UpdateBusNameLabel()
    {
        if (_busNameLabel == null) return;
        _busNameLabel.Text = _busName;
    }

    private void UpdateVolumePercentageLabel()
    {
        _volumePercentageLabel.Text = $"{Mathf.Floor(_volumeSlider.Value)}%";
    }

    private float ConvertPercentageToDecibels(double percentage)
    {
        float scale = 20.0f;
        float divisor = 50.0f;
        return scale * (float)Math.Log10(percentage / divisor);
    }
}
