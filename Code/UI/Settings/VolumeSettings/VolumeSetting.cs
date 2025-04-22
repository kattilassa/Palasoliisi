// VolumeSetting.cs
// Created by Kati Savolainen following a tutorial from youtube with small modifications:
// Tutorial by Cadoink: https://www.youtube.com/watch?v=tkwhNXaBSec
//
// This script provides a reusable UI component for adjusting the volume of an audio bus in Godot.
// This script is reused in music, sound effect and ambience volume sliders.
// User volume settings are saved and loaded from a config file.

using Godot;
using System;
using System.Collections.Generic;
[Tool]
public partial class VolumeSetting : HBoxContainer
{
    private static Dictionary<int, double> _volumePercentageByBusIndex = new Dictionary<int, double>();
    private const string ConfigPath = "user://audio_settings.cfg";
    //Default _busName value for the editor
    private string _busName = "<Bus Name>";
    private int _busIndex;
    private Label _busNameLabel;
    private HSlider _volumeSlider;
    private Label _volumePercentageLabel;
    private AudioStreamPlayer _audioStreamPlayer;
    private Timer _timer = new Timer();

    [Export] public string BusName
    {
        get
        {
            return _busName;
        }
        set
        {
            _busName = value;
            UpdateBusNameLabel();
            //Call UpdateBusNameLabel everytime the busname is set.
        }
    }

    [Export] public int Busindex
    {
         // The index of the audio bus this slider controls.
        get
        {
            return _busIndex;
        }
        set
        {
            _busIndex = value;
        }
    }

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            foreach (var child in GetChildren())
            {
                if (child is AudioStreamPlayer audioStreamPlayerChild)
                {
                    _audioStreamPlayer = audioStreamPlayerChild;
                    //Play a sound for the user to test sound effect volume.
                    AddChild(_timer);
                    _timer.WaitTime = 1.0f;
                    _timer.OneShot = true;
                    //Play the sound only if the time has passed.
                }
            }
        }
        //Get reference to the Label in the scene.
        _busNameLabel = GetNode<Label>("BusNameLabel");
        //Call UpdateBusNameLabel in Ready method to set the busname.
        UpdateBusNameLabel();
        //Get reference to the HSlider in the scene.
        _volumeSlider = GetNode<HSlider>("VolumeHSlider");
        //Connect to the volumesliders ValueChanged event
        _volumeSlider.ValueChanged += _volumeSlider_ValueChanged;
        //Get reference to the volume percentage label in the scene.
        _volumePercentageLabel = GetNode<Label>("VolumePercentageLabel");

        LoadVolumeSettings();
    }

    private void _volumeSlider_ValueChanged(double value)
    {
        UpdateVolumePercentageLabel();
        _volumePercentageByBusIndex[Busindex] = value;
        //Update the dictionary with the latest value.

        if (value == 0)
        {
            AudioServer.SetBusMute(Busindex, true);
            //If volume is zero - mute the bus.
        }
        else
        {
            if (AudioServer.IsBusMute(Busindex))
                AudioServer.SetBusMute(Busindex, false);
            //If the bus is muted but the value is not zero - unmute the bus

            float decibels = ConvertPercentageToDecibels(value);
            //Converts 0–100% value to a dB value.
            AudioServer.SetBusVolumeDb(Busindex, decibels);
            //Set the volume db on the Bus.
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
        config.Load(ConfigPath);

        foreach (var entry in _volumePercentageByBusIndex)
        {
            // Store audio value.
            config.SetValue("Audio", $"Bus_{entry.Key}_volume", entry.Value);

        }

        // Save it to a file (overwrite if already exists).
        config.Save(ConfigPath);
    }

    private void LoadVolumeSettings()
    {
        ConfigFile config = new ConfigFile();
        if (config.Load(ConfigPath) != Error.Ok) return;
        // Load data from a file.
        // If the file didn't load, ignore it.

        if (config.HasSection("Audio"))
        {
            double defaultVolume = 50.0;
            // Fetch the data for saved audio settings.
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
       // If the label exists, set its text to the bus name.
        if (_busNameLabel == null)
        {
            return;
        }
        _busNameLabel.Text = _busName;
    }

    private void UpdateVolumePercentageLabel()
    {
        //Update the volume percentage label with the current slider value.
        _volumePercentageLabel.Text = $"{Mathf.Floor(_volumeSlider.Value)}%";
    }

    private float ConvertPercentageToDecibels(double percentage)
    {
        //Converts 0–100% value to a dB value.
        //This equation converts percentages to decibels.
        //Divisor represents zero decibels and 50% volume percentage.
        //100% volume percentage is 6 decibels.
        float scale = 20.0f;
        float divisor = 50.0f;
        return scale * (float)Math.Log10(percentage / divisor);
        //returns scale multiplied by log of our percentage divided by divisor
    }
}
