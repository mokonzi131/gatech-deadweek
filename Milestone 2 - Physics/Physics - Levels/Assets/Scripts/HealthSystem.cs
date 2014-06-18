using UnityEngine;

class HealthSystem : ScrollBarEssentials
{
    public HealthSystem(Rect sb_dimen, bool vbar, Texture sb_bt, Texture st, float rot) : base(sb_dimen, vbar, sb_bt, st, rot)
    {

    }

    public HealthSystem(Rect sb_dimen, Rect sbs_dimen, bool vbar, Texture sb_bt, Texture st, float rot) : base(sb_dimen, sbs_dimen, vbar, sb_bt, st, rot)
    {

    }

    public void Initialize(int maxVal)
    {
        max_value = DetermineMaxVal(maxVal);
		current_value += max_value;
    }

    public void Update()
    {
        if (current_value < 0)
            current_value = 0;
        else if (current_value >= max_value)
            current_value = max_value;
    }

    public override void IncrimentBar(float value)
    {
        ProcessValue(value);
    }

	public void refill()
	{
		ProcessValue (2000.0f);
	}

    protected override int DetermineMaxVal(int value)
    {
        return base.DetermineMaxVal(value);
    }
}
