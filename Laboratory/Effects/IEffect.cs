namespace Laboratory.Effects;

public interface IEffect
{
    public float Timer { get; set; }
        
    public virtual void Awake() { }
    public virtual void Update() { }
    public virtual void LateUpdate() { }
    public virtual void FixedUpdate() { }
    public virtual void OnDestroy() { }
    public virtual void Cancel() { }
}