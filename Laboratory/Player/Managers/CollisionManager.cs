using System;
using Laboratory.Effects.MonoBehaviours;
using Reactor;
using UnityEngine;

namespace Laboratory.Player.Managers;

[RegisterInIl2Cpp]
public class CollisionManager : MonoBehaviour
{
    public CollisionManager(IntPtr ptr) : base(ptr)
    {
    }

    public PlayerControl Player { get; private set; } = null!;
    public PlayerManager Manager { get; private set; } = null!;
    public PlayerEffectManager EffectsManager { get; private set; } = null!;

    public bool AmOwner => Player.AmOwner;

    private static readonly (string, int)[] _layers = { ("Players", LayerMask.NameToLayer("Players")), ("Objects", LayerMask.NameToLayer("Objects")) };

    public virtual void Awake()
    {
        foreach (var (layerName, layer) in _layers)
        {
            GameObject obj = new($"{nameof(CollisionDetector)} ({layerName})") { layer = layer };
            obj.AddComponent<CollisionDetector>().Parent = this;
            var detectorTransform = obj.transform;
            var box = obj.AddComponent<BoxCollider2D>();
            var body = obj.AddComponent<Rigidbody2D>();

            body.gravityScale = 0f;
            body.isKinematic = true;

            box.size = new Vector2(0.79f, 0.97f);
            box.isTrigger = true;

            detectorTransform.parent = transform;
            detectorTransform.localScale = new Vector3(1, 1, 1);
            detectorTransform.localPosition = Vector3.zero;
        }
    }

    public virtual void Start()
    {
        Player = GetComponent<PlayerControl>();
        Manager = GetComponent<PlayerManager>();
        EffectsManager = GetComponent<PlayerEffectManager>();
    }

    public virtual void OnCollision(Collider2D other)
    {
    }
}

[RegisterInIl2Cpp]
public class CollisionDetector : MonoBehaviour
{
    public CollisionDetector(IntPtr ptr) : base(ptr)
    {
    }

    public CollisionManager Parent { get; set; } = null!;

    // Make a collider that doesn't interact with walls -> { Layer = Objects (11), Trigger = true }
    public void OnTriggerEnter2D(Collider2D other)
    {
        Parent.OnCollision(other);
    }

    // Make a collider that interacts with walls -> { Layer = Players (8), Trigger = false }
    public void OnCollisionEnter2D(Collision2D other)
    {
        Parent.OnCollision(other.collider);
    }
}
