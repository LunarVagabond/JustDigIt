using Godot;

public partial class OptionsMenu : Control
{

	private HSlider masterSlider;
	private HSlider musicSlider;
	private HSlider sfxSlider;
	private LineEdit masterInput;
	private LineEdit musicInput;
	private LineEdit sfxInput;

	public override void _Ready()
	{
		masterSlider = GetNode<HSlider>("%MasterVolumeSlider");
		masterInput = GetNode<LineEdit>("%MasterEntry");
		musicSlider = GetNode<HSlider>("%MusicVolumeSlider");
		musicInput = GetNode<LineEdit>("%MusicEntry");
		sfxSlider = GetNode<HSlider>("%SFXVolumeSlider");
		sfxInput = GetNode<LineEdit>("%SFXEntry");
		WireSignals();
		InitVolumeUI("Master", masterSlider, masterInput);
		InitVolumeUI("Music", musicSlider, musicInput);
		InitVolumeUI("SFX", sfxSlider, sfxInput);
	}

	private void WireSignals()
	{
		masterSlider.ValueChanged += (value) => OnSliderChanged(value, "Master", masterInput);
		musicSlider.ValueChanged += (value) => OnSliderChanged(value, "Music", musicInput);
		sfxSlider.ValueChanged += (value) => OnSliderChanged(value, "SFX", sfxInput);

		// Connect text inputs
		masterInput.TextSubmitted += (text) => OnTextChanged(text, "Master", masterSlider);
		musicInput.TextSubmitted += (text) => OnTextChanged(text, "Music", musicSlider);
		sfxInput.TextSubmitted += (text) => OnTextChanged(text, "SFX", sfxSlider);
	}

	private void InitVolumeUI(string busName, HSlider slider, LineEdit input)
	{
		int busIndex = AudioServer.GetBusIndex(busName);
		float db = AudioServer.GetBusVolumeDb(busIndex);
		float linear = Mathf.DbToLinear(db);

		int volumePercent = Mathf.RoundToInt(linear * 100);
		slider.Value = volumePercent;
		input.Text = volumePercent.ToString();
	}

	private void OnSliderChanged(double value, string busName, LineEdit linkedEntry)
	{
		int percent = Mathf.RoundToInt((float)value);
		linkedEntry.Text = percent.ToString();
		SetBusVolume(busName, percent);
	}

	private void OnTextChanged(string text, string busName, HSlider linkedSlider)
	{
		if (int.TryParse(text, out int percent))
		{
			percent = Mathf.Clamp(percent, 0, 100);
			linkedSlider.Value = percent;
			SetBusVolume(busName, percent);
		}
	}

	private void SetBusVolume(string busName, int percent)
	{
		float linear = Mathf.Clamp(percent / 100.0f, 0.0001f, 1.0f);
		float db = Mathf.LinearToDb(linear);
		int index = AudioServer.GetBusIndex(busName);
		AudioServer.SetBusVolumeDb(index, db);
	}

	private void OnClosePressed() => Visible = false;
	public void ToggleVisable() => Visible = !Visible;
}
