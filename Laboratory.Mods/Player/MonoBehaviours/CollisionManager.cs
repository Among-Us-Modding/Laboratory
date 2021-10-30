using System;
using Laboratory.Mods.Effects.MonoBehaviours;
using Reactor;
using UnityEngine;

namespace Laboratory.Mods.Player.MonoBehaviours
{
    [RegisterInIl2Cpp]
    public class CollisionManager : MonoBehaviour
    {
        public CollisionManager(IntPtr ptr) : base(ptr) { }
        
        public PlayerControl MyPlayer;
        public PlayerManager MyManager;
        public PlayerEffectManager MyEffectsManager;

        public bool AmOwner => MyPlayer.AmOwner;
        
        public virtual void Awake()
        {
            for (int i = 8; i <= 11; i += 3)
            {
                GameObject obj = new(nameof(CollisionDetector) + LayerMask.LayerToName(i)) {layer = i};
                CollisionDetector detector = obj.AddComponent<CollisionDetector>();
                Transform detectorTransform = obj.transform;
                BoxCollider2D box = obj.AddComponent<BoxCollider2D>();
                Rigidbody2D body = obj.AddComponent<Rigidbody2D>();

                body.gravityScale = 0f;
                body.isKinematic = true;
            
                box.size = new Vector2(0.79f, 0.97f);
                box.isTrigger = true;
            
                detectorTransform.parent = transform;
                detectorTransform.localScale = new Vector3(1, 1, 1);
                detectorTransform.localPosition = Vector3.zero;

                detector.Parent = this;
            }
        }

        public virtual void Start()
        {
            MyPlayer = GetComponent<PlayerControl>();
            MyManager = GetComponent<PlayerManager>();
            MyEffectsManager = GetComponent<PlayerEffectManager>();
        }
        
        public virtual void OnCollision(Collider2D other) { }
    }

    [RegisterInIl2Cpp]
    public class CollisionDetector : MonoBehaviour
    {
        public CollisionDetector(IntPtr ptr) : base(ptr) { }

        public CollisionManager Parent;
        
        public void OnTriggerEnter2D(Collider2D other) // Make a collider that doesnt interact with walls -> Layer = Player (8) - Trigger
        {
            Parent.OnCollision(other);
        }

        public void OnCollisionEnter2D(Collision2D other) // Make a collider that interacts with walls -> Layer = Player (8) - Not Trigger
        {
            Parent.OnCollision(other.collider);
        }
    }
}