using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine.Serialization;

namespace MoreMountains.CorgiEngine
{	
	[RequireComponent(typeof(BoxCollider2D))]

    // 면책 조항: 이 컨트롤러는 Corgi Engine을 위해 처음부터 설계되었습니다. 
    // 다양한 방법과 온라인에서 자유롭게 이용할 수 있는 기사들에서 힌트와 영감을 얻었습니다.
    // 특별히 @prime31의 뛰어난 재능과 인내, Yoann Pignole, Mysteriosum, 그리고 Sebastian Lague 등의 
    // 훌륭한 기사와 튜토리얼에 감사를 드립니다. 질문이나 제안이 있으면 unitysupport@reuno.net으로 
    // 언제든지 연락주세요.

    /// <summary>
    /// 캐릭터의 중력과 충돌을 처리하는 캐릭터 컨트롤러입니다.
    /// 이 컨트롤러는 Collider2D와 Rigidbody가 필요합니다.
    /// </summary>
    [AddComponentMenu("Corgi Engine/Character/Core/Corgi Controller")] 
	public class CorgiController : CorgiMonoBehaviour 
	{
		/// the possible modes this controller can update on
		public enum UpdateModes {Update, FixedUpdate}
		
		/// the various states of our character
		public CorgiControllerState State { get; protected set; }

		[Header("기본 파라미터")]

		/// the initial parameters
		[Tooltip("초기화 파라미터")]
		public CorgiControllerParameters DefaultParameters;
		/// the current parameters
		public CorgiControllerParameters Parameters{get{return _overrideParameters ?? DefaultParameters;}}
        
		[Header("충돌 처리")] //Collisions
        [MMInformation("이 캐릭터가 걸을 수 있는 플랫폼, 움직이는 플랫폼 등을 고려할 레이어를 정의해야 합니다. 기본적으로는 Platforms, MovingPlatforms, OneWayPlatforms, MovingOneWayPlatforms 순서로 설정하는 것이 좋습니다.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]
        //You need to define what layer(s) this character will consider a walkable platform/moving platform etc. By default, you want Platforms, MovingPlatforms, OneWayPlatforms, MovingOneWayPlatforms, in this order.

        /// The layer mask the platforms are on
        [Tooltip("플랫폼이 있는 레이어 마스크 The layer mask the platforms are on")]
		public LayerMask PlatformMask = LayerManager.PlatformsLayerMask | LayerManager.PushablesLayerMask; 
		/// The layer mask the moving platforms are on
		[Tooltip("움직이는 플랫폼(moving platforms)에 대한 레이어 마스크 설정과 사용 방법 ")] //The layer mask the moving platforms are on
        public LayerMask MovingPlatformMask = LayerManager.MovingPlatformsLayerMask; 
		/// The layer mask the one way platforms are on
		[Tooltip("한방향 플랫폼이 있는 레이어 마스크")]
		public LayerMask OneWayPlatformMask = LayerManager.OneWayPlatformsLayerMask;
		/// The layer mask the moving one way platforms are on
		[Tooltip("움직이는 일방향 플랫폼이 있는 레이어 마스크")]
		public LayerMask MovingOneWayPlatformMask = LayerManager.MovingOneWayPlatformsMask;
		/// The layer mask the mid height one way paltforms are on
		[Tooltip("중간 높이 일방향 플랫폼이 있는 레이어 마스크")]
		public LayerMask MidHeightOneWayPlatformMask = LayerManager.MidHeightOneWayPlatformsLayerMask;
		/// The layer mask the stairs are on
		[Tooltip("계단이 있는 레이어 마스크")]

        public LayerMask StairsMask = LayerManager.StairsLayerMask;
		/// the possible directions a ray can be cast
		public enum RaycastDirections { up, down, left, right };
		/// The possible ways a character can detach from a oneway or moving platform
		public enum DetachmentMethods { Layer, Object, LayerChange }
		/// When a character jumps from a oneway or moving platform, collisions are off for a short moment. You can decide if they should happen on a whole moving/1way platform layer basis or just with the object the character just left
		[Tooltip("캐릭터가 일방향 또는 움직이는 플랫폼에서 점프할 때, 충돌이 잠시 동안 비활성화됩니다. 이 충돌이 전체 움직이는/일방향 플랫폼 레이어에 대해 발생할지, 아니면 캐릭터가 방금 떠난 오브젝트에만 적용될지를 선택할 수 있습니다")]
		public DetachmentMethods DetachmentMethod = DetachmentMethods.Layer;
		/// When using the LayerChange detachment method, this is the layer one way and moving platforms will be moved to temporarily
		[Tooltip("LayerChange 분리 방식을 사용할 때, 이는 일방향 및 움직이는 플랫폼이 임시로 이동할 레이어를 나타냅니다.")]
		public MMLayer DetachmentLayer;

		/// gives you the object the character is standing on
		[Tooltip("캐릭터가 서 있는 오브젝트를 반환합니다")]
		[MMReadOnly]
		public GameObject StandingOn;
		/// the object the character was standing on last frame
		public GameObject StandingOnLastFrame { get; protected set; }
		/// gives you the collider the character is standing on
		public Collider2D StandingOnCollider { get; protected set; }	
		/// gives you the collider the character is standing on
		public GameObject LastStandingOn { get; protected set; }	
		/// the current speed of the character
		public Vector2 Speed { get{ return _speed; } }
		/// the world speed of the character
		public virtual Vector2 WorldSpeed { get { return _worldSpeed; } }
		/// the value of the forces applied at one point in time 
		public virtual Vector2 ForcesApplied { get; protected set; }
		/// the wall we're currently colliding with
		public virtual GameObject CurrentWallCollider { get; protected set; }

		[Header("레이캐스팅")]
		[MMInformation(
            "여기에서 가로 및 세로로 발사되는 레이의 개수를 정의할 수 있습니다. 레이 간의 거리를 가능한 멀리 설정하되, 두 레이 사이에 장애물이나 적이 들어갈 수 없을 정도로 충분히 가깝게 설정해야 합니다.",
			MoreMountains.Tools.MMInformationAttribute.InformationType.Info, false)]
		/// whether this controller should run on Update or FixedUpdate
		[Tooltip("이 컨트롤러가 Update에서 실행될지 FixedUpdate에서 실행될지를 설정합니다")]
		public UpdateModes UpdateMode = UpdateModes.Update;
		/// the number of rays cast horizontally
		[Tooltip("가로로 발사되는 레이의 개수")]
		public int NumberOfHorizontalRays = 8;
		/// the number of rays cast vertically
		[Tooltip("세로로 발사되는 레이의 개수")]
		public int NumberOfVerticalRays = 8;
		/// a small value added to all raycasts to accomodate for edge cases	
		[FormerlySerializedAs("RayOffset")]
		[Tooltip("가로 레이캐스트에 경계 조건을 처리하기 위해 추가되는 작은 값")]
		public float RayOffsetHorizontal = 0.05f;
		/// a small value added to all raycasts to accomodate for edge cases	
		[Tooltip("세로 레이캐스트에 경계 조건을 처리하기 위해 추가되는 작은 값")]
		public float RayOffsetVertical = 0.05f;
		/// an extra length you an add when casting rays horizontally
		[Tooltip("가로로 레이를 발사할 때 추가로 더할 수 있는 길이")]
		public float RayExtraLengthHorizontal = 0f;
		/// an extra length you an add when casting rays vertically
		[Tooltip("세로로 레이를 발사할 때 추가로 더할 수 있는 길이")]
		public float RayExtraLengthVertical = 0f;
		
		/// by default, the length of the raycasts used to get back to normal size will be auto generated based on your character's normal/standing height, but here you can specify a different value
		[Tooltip("기본적으로, 원래 크기로 돌아가기 위해 사용되는 레이캐스트의 길이는 캐릭터의 정상/서 있는 높이를 기준으로 자동 생성되지만, 여기에서 다른 값을 지정할 수 있습니다")]
		public float CrouchedRaycastLengthMultiplier = 1f;
		/// if this is true, rays will be cast on both sides, otherwise only in the current movement's direction.
		[Tooltip("이 값이 true이면 레이가 양쪽으로 발사되고, 그렇지 않으면 현재 이동 방향으로만 발사됩니다.")]
		public bool CastRaysOnBothSides = false;
		/// the maximum length of the ray used to detect the distance to the ground
		[Tooltip("땅까지의 거리를 감지하는 데 사용되는 레이의 최대 길이")]
		public float DistanceToTheGroundRayMaximumLength = 100f;
		/// a multiplier to apply to vertical downward raycasts while on a moving platform (longer == more stable movement on platforms)
		[Tooltip("움직이는 플랫폼 위에서 아래쪽으로 향하는 세로 레이캐스트에 적용되는 곱셈 계수 (더 길수록 플랫폼 위에서의 움직임이 더 안정적임)")]
		public float OnMovingPlatformRaycastLengthMultiplier = 2f;
		/// an offset to apply vertically to the origin of the controller's raycasts that will have an impact on obstacle detection. Tweak this to adapt to your character's and obstacle's size.
		[Tooltip("컨트롤러의 레이캐스트 시작점에 세로로 적용되는 오프셋으로, 장애물 감지에 영향을 미칩니다. 캐릭터와 장애물의 크기에 맞게 조정하세요.")]
		public float ObstacleHeightTolerance = 0.05f;
		
		[Header("Stickiness 접착성")]
		[MMInformation("여기에서 캐릭터가 경사를 내려갈 때 경사면에 붙도록 할지 여부와 이를 처리하는 레이캐스트의 길이를 정의할 수 있습니다. (0은 자동 길이를 의미합니다)", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// If this is set to true, the character will stick to slopes when walking down them
		[Tooltip("이 값이 true로 설정되면, 캐릭터가 경사를 내려갈 때 경사면에 붙습니다.")]
		public bool StickToSlopes = false;
		/// The length of the raycasts used to stick to downward slopes
		[Tooltip("경사를 내려갈 때 경사면에 붙는 데 사용되는 레이캐스트의 길이")]
		public float StickyRaycastLength = 0f;
		/// the movement's Y offset to evaluate for stickiness. 
		[Tooltip("접착성을 평가하기 위한 이동의 Y 오프셋")]
		public float StickToSlopesOffsetY = 0.2f;
		/// the time (in seconds) since the last time the character was grounded 
		[Tooltip("캐릭터가 마지막으로 땅에 닿은 이후 경과 시간(초)")]
		[MMReadOnly]
		public float TimeAirborne = 0f;

		
		[Header("Safety 안전성")]
		[MMInformation("여기에서 컨트롤러가 회전된 상태로 시작하도록 허용할 수 있습니다. 이는 중력 방향을 변경합니다. 이 안전 설정을 활성화 상태로 두고 CharacterGravity 기능을 사용하는 것이 더 안전합니다.", MoreMountains.Tools.MMInformationAttribute.InformationType.Info,false)]

		/// whether or not to perform additional checks when setting the transform's position (typically done by abilities like CharacterGrip). Slightly more expensive in terms of performance, but also safer. 
		[Tooltip("Transform의 위치를 설정할 때 추가 확인을 수행할지 여부 (주로 CharacterGrip 같은 기능에 의해 수행됨). 성능 면에서는 약간 더 부담되지만, 더 안전합니다")]
		public bool SafeSetTransform = false;
		/// if this is true, this controller will set a number of physics settings automatically on init, to ensure they're correct
		[Tooltip("이 값이 true이면, 이 컨트롤러는 초기화 시 자동으로 여러 물리 설정을 적용하여 올바르게 설정되었는지 확인합니다.")]
		public bool AutomaticallySetPhysicsSettings = true;
		
		/// if this is true, gravity ability settings will be automatically set. It's recommended to set that to true.
		[Tooltip("이 값이 true이면, 중력 기능 설정이 자동으로 적용됩니다. true로 설정하는 것이 권장됩니다.")]
		public bool AutomaticGravitySettings = true;
		
		
		/// if this is true, the controller will perform an extra boxcast before authorizing movement, from the previous place to the next.
		/// If an obstacle is found between the two, movement will be prevented, and extraction will be attempted
		/// In most contexts this can remain false, but if you experience characters going through platforms, turn it on 
		[Tooltip("이 값이 true이면, 컨트롤러는 이전 위치에서 다음 위치로 이동을 허용하기 전에 추가 박스캐스트를 수행합니다. " +
                 "두 위치 사이에 장애물이 발견되면 이동이 차단되고 탈출이 시도됩니다." +
                 "대부분의 상황에서는 false로 유지해도 되지만, 캐릭터가 플랫폼을 통과하는 문제가 발생하면 이 옵션을 활성화하세요")]
		public bool PerformSafetyBoxcast = false;
		/// whether or not to only perform the safety boxcast check in the air
		[Tooltip("안전한 박스캐스트 확인을 공중에서만 수행할지 여부")]
		[MMCondition("PerformSafetyBoxcast", true)]
		public bool SafetyBoxcastInAirOnly = true;
		/// the ratio to apply to the Character's size when boxcasting. You'll want it to be always smaller than the actual bounds. 0.8, or 0.5 are good default values in most contexts
		[Tooltip("박스캐스트 시 캐릭터 크기에 적용할 비율입니다. 항상 실제 경계보다 작아야 합니다. 대부분의 상황에서는 0.8 또는 0.5가 적절한 기본값입니다")]
		[MMCondition("PerformSafetyBoxcast", true)]
		public Vector2 SafetyBoxcastSizeRatio = new Vector2(0.8f, 0.8f);
		/// if an obstacle is found, extraction will be attempted, and the controller will move back along its last movement line, trying to find a safe space to put the character.
		/// This defines the distance of each of these moves. Keep it small, and consistent with your character's size and speed
		[Tooltip("장애물이 발견되면 탈출이 시도되며, 컨트롤러는 마지막 이동 경로를 따라 되돌아가면서 캐릭터를 위한 안전한 공간을 찾으려고 합니다 " +
                 "이 값은 각 이동의 거리를 정의합니다. 작게 설정하고, 캐릭터의 크기와 속도에 맞게 유지하세요.")]
		[MMCondition("PerformSafetyBoxcast", true)]
		public float ExtractionIncrement = 0.05f;
		/// The amount of times the controller should try to extract from a potential non safe space
		[Tooltip("컨트롤러가 잠재적으로 안전하지 않은 공간에서 탈출을 시도할 횟수")]
		[MMCondition("PerformSafetyBoxcast", true)]
		public int MaxExtractionsIterations = 10;

		public Vector3 ColliderSize => Vector3.Scale(transform.localScale, _boxCollider.size);
		public Vector2 ColliderOffset => _boxCollider.offset;
		public Vector3 ColliderCenterPosition => _boxCollider.bounds.center;

		public virtual Vector3 ColliderBottomPosition { get
		{
			_colliderBottomCenterPosition.x = _boxCollider.bounds.center.x;
			_colliderBottomCenterPosition.y = _boxCollider.bounds.min.y;
			_colliderBottomCenterPosition.z = 0;
			return _colliderBottomCenterPosition;
		}}

		public virtual Vector3 ColliderLeftPosition { get
		{
			_colliderLeftCenterPosition.x = _boxCollider.bounds.min.x;
			_colliderLeftCenterPosition.y = _boxCollider.bounds.center.y;
			_colliderLeftCenterPosition.z = 0;
			return _colliderLeftCenterPosition;
		}}

		public virtual Vector3 ColliderTopPosition { get
		{
			_colliderTopCenterPosition.x = _boxCollider.bounds.center.x;
			_colliderTopCenterPosition.y = _boxCollider.bounds.max.y;
			_colliderTopCenterPosition.z = 0;
			return _colliderTopCenterPosition;
		}}

		public virtual Vector3 ColliderRightPosition { get
		{
			_colliderRightCenterPosition.x = _boxCollider.bounds.max.x;
			_colliderRightCenterPosition.y =  _boxCollider.bounds.center.y;
			_colliderRightCenterPosition.z = 0;
			return _colliderRightCenterPosition;
		}}
		
		/// Is gravity active? 
		public bool IsGravityActive => _gravityActive; 
		public virtual float DeltaTime => _update ? Time.deltaTime : Time.fixedDeltaTime;  
		public float Friction => _friction; 
		public SurfaceModifier CurrentSurfaceModifier { get; set; }
		public GameObject[] StandingOnArray { get; set; }

		/// <summary>
		/// Returns the character's bounds width
		/// </summary>
		public virtual float Width()
		{
			return _boundsWidth;
		}

		/// <summary>
		/// Returns the character's bounds height
		/// </summary>
		public virtual float Height()
		{
			return _boundsHeight;
		}

		public virtual Vector2 Bounds { get
			{
				_bounds.x = _boundsWidth;
				_bounds.y = _boundsHeight;
				return _bounds;
			}
		}
        
		public virtual Vector3 BoundsTopLeftCorner { get
		{
			return _boundsTopLeftCorner;
		}}

		public virtual Vector3 BoundsBottomLeftCorner { get
		{
			return _boundsBottomLeftCorner;
		}}

		public virtual Vector3 BoundsTopRightCorner { get
		{
			return _boundsTopRightCorner;
		}}

		public virtual Vector3 BoundsBottomRightCorner { get
		{
			return _boundsBottomRightCorner;
		}}

		public virtual Vector3 BoundsTop { get
		{
			return (_boundsTopLeftCorner + _boundsTopRightCorner)/2 ;
		}}

		public virtual Vector3 BoundsBottom { get
		{
			return (_boundsBottomLeftCorner + _boundsBottomRightCorner)/2 ;
		}}

		public virtual Vector3 BoundsRight { get
		{
			return (_boundsTopRightCorner + _boundsBottomRightCorner)/2 ;
		}}

		public virtual Vector3 BoundsLeft { get
		{
			return (_boundsTopLeftCorner + _boundsBottomLeftCorner)/2 ;
		}}
        
		public virtual Vector3 BoundsCenter { get
		{
			return _boundsCenter;
		}}
        
		public virtual float DistanceToTheGround { get
		{
			return _distanceToTheGround;
		}}

		public virtual Vector2 ExternalForce
		{
			get
			{ 
				return _externalForce;
			}
		}
        
		// parameters override storage
		protected CorgiControllerParameters _overrideParameters;
		// private local references			
		protected Vector2 _speed;
		protected float _friction=0;
		protected float _fallSlowFactor; 
		protected float _currentGravity = 0;
		protected Vector2 _externalForce;
		protected Vector2 _newPosition;
		protected Transform _transform;
		protected BoxCollider2D _boxCollider;
		protected CharacterGravity _characterGravity;
		protected LayerMask _platformMaskSave;
		protected LayerMask _raysBelowLayerMaskPlatforms;
		protected LayerMask _raysBelowLayerMaskPlatformsWithoutOneWay;
		protected LayerMask _raysBelowLayerMaskPlatformsWithoutMidHeight;
		protected int _savedBelowLayer;
		protected MMPathMovement _movingPlatform = null;
		protected MMPathMovement _pusherPlatform = null;
		protected float _movingPlatformCurrentGravity;
		protected bool _gravityActive=true;
		protected Collider2D _ignoredCollider = null;
		protected bool _collisionsOnWithStairs = false;

		protected const float _smallValue=0.0001f;
		protected const float _movingPlatformsGravity=-500;

		protected Vector2 _originalColliderSize;
		protected Vector2 _originalColliderOffset;
		protected Vector2 _originalSizeRaycastOrigin;

		protected Vector3 _crossBelowSlopeAngle;

		protected RaycastHit2D[] _sideHitsStorage;	
		protected RaycastHit2D[] _belowHitsStorage;	
		protected RaycastHit2D[] _aboveHitsStorage;	
		protected RaycastHit2D _stickRaycastLeft;
		protected RaycastHit2D _stickRaycastRight;
		protected RaycastHit2D _stickRaycast;
		protected RaycastHit2D _distanceToTheGroundRaycast;
		protected float _movementDirection;
		protected float _storedMovementDirection = 1;
		protected const float _movementDirectionThreshold = 0.0001f;

		protected Vector2 _horizontalRayCastFromBottom = Vector2.zero;
		protected Vector2 _horizontalRayCastToTop = Vector2.zero;
		protected Vector2 _verticalRayCastFromLeft = Vector2.zero;
		protected Vector2 _verticalRayCastToRight = Vector2.zero;
		protected Vector2 _aboveRayCastStart = Vector2.zero;
		protected Vector2 _aboveRayCastEnd = Vector2.zero;
		protected Vector2 _rayCastOrigin = Vector2.zero;

		protected Vector3 _colliderBottomCenterPosition;
		protected Vector3 _colliderLeftCenterPosition;
		protected Vector3 _colliderRightCenterPosition;
		protected Vector3 _colliderTopCenterPosition;

		protected MMPathMovement _movingPlatformTest;
		protected SurfaceModifier _frictionTest;
		protected bool _update;

		protected RaycastHit2D[] _raycastNonAlloc = new RaycastHit2D[0];

		protected Vector2 _boundsTopLeftCorner;
		protected Vector2 _boundsBottomLeftCorner;
		protected Vector2 _boundsTopRightCorner;
		protected Vector2 _boundsBottomRightCorner;
		protected Vector2 _boundsCenter;
		protected Vector2 _bounds;
		protected float _boundsWidth;
		protected float _boundsHeight;
		protected float _distanceToTheGround;
		protected Vector2 _worldSpeed;

		protected List<RaycastHit2D> _contactList;
		protected bool _shouldComputeNewSpeed = false;
		protected Vector2 _safetyExtractionSize = Vector2.zero;

		/// <summary>
		/// initialization
		/// </summary>
		protected virtual void Awake()
		{
			Initialization ();
		}

		protected virtual void Initialization()
		{
			// we get the various components
			_transform = transform;
			_boxCollider = this.gameObject.GetComponent<BoxCollider2D>();
			_originalColliderSize = _boxCollider.size;
			_originalColliderOffset = _boxCollider.offset;
			CurrentSurfaceModifier = null;
			
			// we test the boxcollider's x offset. If it's not null we trigger a warning.
			if ((_boxCollider.offset.x!=0) && (Parameters.DisplayWarnings))
			{
				Debug.LogWarning("The boxcollider for "+gameObject.name+" should have an x offset set to zero. Right now this may cause issues when you change direction close to a wall.");
			}

			// raycast list and state init
			_contactList = new List<RaycastHit2D>();
			State = new CorgiControllerState();

			// we add the edge collider platform and moving platform masks to our initial platform mask so they can be walked on	
			CachePlatformMask();
			PlatformMask |= OneWayPlatformMask;
			PlatformMask |= MovingPlatformMask;
			PlatformMask |= MovingOneWayPlatformMask;
			PlatformMask |= MidHeightOneWayPlatformMask;
            
			_sideHitsStorage = new RaycastHit2D[NumberOfHorizontalRays];	
			_belowHitsStorage = new RaycastHit2D[NumberOfVerticalRays];	
			_aboveHitsStorage = new RaycastHit2D[NumberOfVerticalRays];
			_update = (UpdateMode == UpdateModes.Update);

			StandingOnArray = new GameObject[NumberOfVerticalRays];

			CurrentWallCollider = null;
			State.Reset();
			SetRaysParameters();

			ApplyGravitySettings();
			ApplyPhysicsSettings();
		}

		/// <summary>
		/// Forces a number of physics settings, if needed
		/// </summary>
		protected virtual void ApplyPhysicsSettings()
		{
			if (AutomaticallySetPhysicsSettings)
			{
				Physics2D.queriesHitTriggers = true;
				Physics2D.queriesStartInColliders = true;
				Physics2D.callbacksOnDisable = true;
				Physics2D.reuseCollisionCallbacks = true;
				Physics2D.autoSyncTransforms = false;

				if (PlatformMask.MMContains(this.gameObject.layer))
				{
					Physics2D.queriesStartInColliders = false;
				}
			}
		}

		/// <summary>
		/// Forces rotation if we don't have a gravity ability
		/// </summary>
		protected virtual void ApplyGravitySettings()
		{
			if (AutomaticGravitySettings)
			{
				_characterGravity = this.gameObject.MMGetComponentNoAlloc<Character>()?.FindAbility<CharacterGravity>();
				if (_characterGravity == null)
				{
					this.transform.rotation = Quaternion.identity;
				}
			}
		}

		/// <summary>
		/// Use this to add force to the character
		/// </summary>
		/// <param name="force">Force to add to the character.</param>
		public virtual void AddForce(Vector2 force)
		{
			_speed += force;	
			_externalForce += force;
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		///  use this to set the horizontal force applied to the character
		/// </summary>
		/// <param name="x">The x value of the velocity.</param>
		public virtual void AddHorizontalForce(float x)
		{
			_speed.x += x;
			_externalForce.x += x;
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		///  use this to set the vertical force applied to the character
		/// </summary>
		/// <param name="y">The y value of the velocity.</param>
		public virtual void AddVerticalForce(float y)
		{
			_speed.y += y;
			_externalForce.y += y;
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		/// Use this to set the force applied to the character
		/// </summary>
		/// <param name="force">Force to apply to the character.</param>
		public virtual void SetForce(Vector2 force)
		{
			_speed = force;
			_externalForce = force;	
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		///  use this to set the horizontal force applied to the character
		/// </summary>
		/// <param name="x">The x value of the velocity.</param>
		public virtual void SetHorizontalForce (float x)
		{
			_speed.x = x;
			_externalForce.x = x;
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		///  use this to set the vertical force applied to the character
		/// </summary>
		/// <param name="y">The y value of the velocity.</param>
		public virtual void SetVerticalForce (float y)
		{
			_speed.y = y;
			_externalForce.y = y;
			ClampSpeed();
			ClampExternalForce();
		}

		/// <summary>
		/// A method you can use to cache the PlatformMask should you change it
		/// </summary>
		public virtual void CachePlatformMask()
		{
			_platformMaskSave = PlatformMask;	
		}

		/// <summary>
		/// On FixedUpdate we run our routine if needed 
		/// </summary>
		protected virtual void FixedUpdate()
		{
			if (!_update)
			{
				EveryFrame();
			}
		}

		/// <summary>
		/// On Update we run our routine if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_update)
			{
				EveryFrame();	
			}
		}

		/// <summary>
		/// On late update we reset our airborne time
		/// </summary>
		protected virtual void LateUpdate()
		{
			TimeAirborne = (State.IsGrounded) ? 0f : TimeAirborne + DeltaTime;
		}

		/// <summary>
		/// Every frame, we apply the gravity to our character, then check using raycasts if an object's been hit, and modify its new position 
		/// accordingly. When all the checks have been done, we apply that new position. 
		/// </summary>
		protected virtual void EveryFrame()
		{
			if (Time.timeScale == 0f)
			{
				return;
			}

			ApplyGravity ();
			FrameInitialization ();

			// we initialize our rays
			SetRaysParameters();
			HandleMovingPlatforms(_movingPlatform);
			HandleMovingPlatforms(_pusherPlatform, true);

			// we store our current speed for use in moving platforms mostly
			ForcesApplied = _speed;

			// we cast rays on all sides to check for slopes and collisions
			DetermineMovementDirection();
			if (CastRaysOnBothSides)
			{
				CastRaysToTheLeft();
				CastRaysToTheRight();
			}
			else
			{                
				if (_movementDirection == -1)
				{
					CastRaysToTheLeft();
				}
				else
				{
					CastRaysToTheRight();
				}
			}
			CastRaysBelow();	
			CastRaysAbove();

			MoveTransform();		

			SetRaysParameters();	
			ComputeNewSpeed ();            
			SetStates ();
			ComputeDistanceToTheGround();

			_externalForce.x=0;
			_externalForce.y=0;

			FrameExit();

			_worldSpeed = Speed;
		}

		protected virtual void FrameInitialization()
		{
			_contactList.Clear();
			// we initialize our newposition, which we'll use in all the next computations			
			_newPosition = Speed * DeltaTime;
			State.WasGroundedLastFrame = State.IsCollidingBelow;          
			StandingOnLastFrame = StandingOn;
			State.WasTouchingTheCeilingLastFrame = State.IsCollidingAbove;
			CurrentWallCollider = null;
			_shouldComputeNewSpeed = true;
			State.Reset(); 
		}

		/// <summary>
		/// Called at the very last moment
		/// </summary>
		protected virtual void FrameExit()
		{
			// on frame exit we put our standing on last frame object back to where it belongs
			if (StandingOnLastFrame != null)
			{
				StandingOnLastFrame.layer = _savedBelowLayer;
			}
		}

		/// <summary>
		/// Determines the current movement direction
		/// </summary>
		protected virtual void DetermineMovementDirection()
		{
			_movementDirection = _storedMovementDirection;
			if (_speed.x < -_movementDirectionThreshold)
			{
				_movementDirection = -1;
			} else if (_speed.x > _movementDirectionThreshold)
			{
				_movementDirection = 1;
			} else if (_externalForce.x < -_movementDirectionThreshold)
			{
				_movementDirection = -1;
			} else if (_externalForce.x > _movementDirectionThreshold)
			{
				_movementDirection = 1;
			}

			ApplyMovingPlatformMovementDirection(_movingPlatform);
			ApplyMovingPlatformMovementDirection(_pusherPlatform);
			
			_storedMovementDirection = _movementDirection;                        
		}

		public virtual void SetPusherPlatform(MMPathMovement newPusherPlatform)
		{
			_pusherPlatform = newPusherPlatform;
		}

		protected virtual void ApplyMovingPlatformMovementDirection(MMPathMovement movingPlatform)
		{
			if (movingPlatform != null)
			{
				if (Mathf.Abs(movingPlatform.CurrentSpeed.x) > Mathf.Abs(_speed.x))
				{
					_movementDirection = Mathf.Sign(movingPlatform.CurrentSpeed.x);
				}
			}
		}

		/// <summary>
		/// Moves the transform to its new position, after having performed an optional safety boxcast
		/// </summary>
		protected virtual void MoveTransform()
		{
			// we move our transform to its next position
			RunSafetyBoxcast();
			
			_transform.Translate(_newPosition, Space.Self);
		}

		/// <summary>
		/// Returns true if a safety extraction is required, false otherwise
		/// </summary>
		/// <returns></returns>
		protected virtual void RunSafetyBoxcast()
		{
			if (!PerformSafetyBoxcast)
			{
				return;
			}

			if (SafetyBoxcastInAirOnly && !State.IsGrounded)
			{
				return;
			}

			_safetyExtractionSize.x = Bounds.x * SafetyBoxcastSizeRatio.x - RayOffsetHorizontal;
			_safetyExtractionSize.y = Bounds.y * SafetyBoxcastSizeRatio.y - RayOffsetVertical;
			_stickRaycast = MMDebug.BoxCast(_boundsCenter, _safetyExtractionSize, Vector2.Angle(transform.up, Vector2.up), _newPosition.normalized, _newPosition.magnitude, 
				PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, Color.yellow, true);

			if (_stickRaycast)
			{
				int i = 1;
				bool extractionFound = false;

				while (i < MaxExtractionsIterations && !extractionFound)
				{
					_newPosition = _newPosition.normalized * (_stickRaycast.distance - ExtractionIncrement);
					_safetyExtractionSize.x = Bounds.x - RayOffsetHorizontal;
					_safetyExtractionSize.y = Bounds.y - RayOffsetVertical;
					_stickRaycast = MMDebug.BoxCast(_boundsCenter, _safetyExtractionSize, Vector2.Angle(transform.up, Vector2.up), _newPosition.normalized, _newPosition.magnitude, 
						PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, Color.yellow, true);
					if (!_stickRaycast)
					{
						extractionFound = true;
					}
					i++;
				}
				
			}
		}

		/// <summary>
		/// Applies gravity to the current speed
		/// </summary>
		protected virtual void ApplyGravity()
		{
			_currentGravity = Parameters.Gravity;
			if (_speed.y > 0)
			{
				_currentGravity = _currentGravity / Parameters.AscentMultiplier;
			}
			if (_speed.y < 0)
			{
				_currentGravity = _currentGravity * Parameters.FallMultiplier;
			}
			if (_gravityActive)
			{
				float fallSlowFactor = (_fallSlowFactor != 0) ? _fallSlowFactor : 1f;
				_speed.y += (_currentGravity + _movingPlatformCurrentGravity) * fallSlowFactor * DeltaTime;
			}
		}

		/// <summary>
		/// If the CorgiController is standing on a moving platform, we match its speed
		/// </summary>
		protected virtual void HandleMovingPlatforms(MMPathMovement movingPlatform, bool pushingPlatform = false)
		{
			if ((movingPlatform != null) && (movingPlatform.enabled))			
			{
				if (!float.IsNaN(movingPlatform.CurrentSpeed.x) && !float.IsNaN(movingPlatform.CurrentSpeed.y) && !float.IsNaN(movingPlatform.CurrentSpeed.z))
				{
					_transform.Translate(this.transform.rotation * movingPlatform.CurrentSpeed * DeltaTime);
				}

				if ( (Time.timeScale == 0) || float.IsNaN(movingPlatform.CurrentSpeed.x) || float.IsNaN(movingPlatform.CurrentSpeed.y) || float.IsNaN(movingPlatform.CurrentSpeed.z) )
				{
					return;
				}

				if ((DeltaTime<=0))
				{
					return;
				}

				if (State.WasTouchingTheCeilingLastFrame)
				{
					return;
				}

				if (!pushingPlatform)
				{
					State.OnAMovingPlatform = true;
					GravityActive(false);
					_movingPlatformCurrentGravity = _movingPlatformsGravity;
					_newPosition.y = movingPlatform.CurrentSpeed.y * DeltaTime;	
				}

				_speed = - _newPosition / DeltaTime;	
				_speed.x = -_speed.x;

				SetRaysParameters();
			}
		}

		/// <summary>
		/// Disconnects the CorgiController from its current moving platform.
		/// </summary>
		public virtual void DetachFromMovingPlatform()
		{
			if (_movingPlatform == null)
			{
				return;
			}
			GravityActive(true);
			State.OnAMovingPlatform=false;
			_movingPlatform = null;
			_movingPlatformCurrentGravity=0;
		}

		/// <summary>
		/// A public API to cast rays to any of the 4 cardinal directions using the built-in setup.
		/// You can specify the length and color of your rays, and pass a storageArray as a ref parameter, and then analyse its contents to do whatever you want.
		/// Note that in most situations (other than collision detection) this may be a bit overkill and maybe you should consider casting a single ray instead. It's up to you!
		/// Will return true if any of the rays hit something, false otherwise
		/// </summary>
		/// <returns><c>true</c>, if one of the rays hit something, <c>false</c> otherwise.</returns>
		/// <param name="direction">Direction.</param>
		/// <param name="rayLength">Ray length.</param>
		/// <param name="color">Color.</param>
		/// <param name="storageArray">Storage array.</param>
		public virtual bool CastRays(RaycastDirections direction, float rayLength, Color color, ref RaycastHit2D[] storageArray)
		{
			bool returnValue = false;
			switch (direction)
			{
				case RaycastDirections.left: 
					// we determine the origin of our rays
					_horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
					_horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;	
					_horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector2)transform.up * ObstacleHeightTolerance;
					_horizontalRayCastToTop = _horizontalRayCastToTop - (Vector2)transform.up * ObstacleHeightTolerance;
					for (int i = 0; i < NumberOfHorizontalRays; i++)
					{	
						Vector2 rayOriginPoint = Vector2.Lerp (_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(NumberOfHorizontalRays - 1));
						storageArray [i] = MMDebug.RayCast (rayOriginPoint, -transform.right, rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color, Parameters.DrawRaycastsGizmos);	
						if (storageArray [i])
						{
							returnValue = true;
						}
					}
					return returnValue;

				case RaycastDirections.right:
					// we determine the origin of our rays
					_horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
					_horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;	
					_horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector2)transform.up * ObstacleHeightTolerance;
					_horizontalRayCastToTop = _horizontalRayCastToTop - (Vector2)transform.up * ObstacleHeightTolerance;
					for (int i = 0; i < NumberOfHorizontalRays; i++)
					{	
						Vector2 rayOriginPoint = Vector2.Lerp (_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(NumberOfHorizontalRays - 1));
						storageArray[i] = MMDebug.RayCast (rayOriginPoint, transform.right, rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color, Parameters.DrawRaycastsGizmos);	
						if (storageArray [i])
						{
							returnValue = true;
						}
					}
					return returnValue;

				case RaycastDirections.down:
					// we determine the origin of our rays
					_verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
					_verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	
					_verticalRayCastFromLeft += (Vector2)transform.up * RayOffsetVertical;
					_verticalRayCastToRight += (Vector2)transform.up * RayOffsetVertical;
					_verticalRayCastFromLeft += (Vector2)transform.right * _newPosition.x;
					_verticalRayCastToRight += (Vector2)transform.right * _newPosition.x;
					for (int i = 0; i < NumberOfVerticalRays; i++)
					{			
						Vector2 rayOriginPoint = Vector2.Lerp (_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(NumberOfVerticalRays - 1));

						if ((_newPosition.y < 0) && (!State.WasGroundedLastFrame))
						{
							storageArray [i] = MMDebug.RayCast (rayOriginPoint, -transform.up, rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color, Parameters.DrawRaycastsGizmos);	
							if (storageArray [i])
							{
								returnValue = true;
							}
						}
					}
					return returnValue;

				case RaycastDirections.up:
					// we determine the origin of our rays
					_verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
					_verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	
					_verticalRayCastFromLeft += (Vector2)transform.up * RayOffsetVertical;
					_verticalRayCastToRight += (Vector2)transform.up * RayOffsetVertical;
					_verticalRayCastFromLeft += (Vector2)transform.right * _newPosition.x;
					_verticalRayCastToRight += (Vector2)transform.right * _newPosition.x;
					for (int i = 0; i < NumberOfVerticalRays; i++)
					{			
						Vector2 rayOriginPoint = Vector2.Lerp (_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(NumberOfVerticalRays - 1));

						if ((_newPosition.y > 0) && (!State.WasGroundedLastFrame))
						{
							storageArray [i] = MMDebug.RayCast (rayOriginPoint, transform.up, rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, color, Parameters.DrawRaycastsGizmos);	
							if (storageArray [i])
							{
								returnValue = true;
							}
						}
					}
					return returnValue;

				default:
					return false;
			}
		}

		protected virtual void CastRaysToTheLeft()
		{
			CastRaysToTheSides(-1);
		}

		protected virtual void CastRaysToTheRight()
		{
			CastRaysToTheSides(1);
		}

		/// <summary>
		/// Casts rays to the sides of the character, from its center axis.
		/// If we hit a wall/slope, we check its angle and move or not according to it.
		/// </summary>
		protected virtual void CastRaysToTheSides(float raysDirection) 
		{	
			// we determine the origin of our rays
			_horizontalRayCastFromBottom = (_boundsBottomRightCorner + _boundsBottomLeftCorner) / 2;
			_horizontalRayCastToTop = (_boundsTopLeftCorner + _boundsTopRightCorner) / 2;	
			_horizontalRayCastFromBottom = _horizontalRayCastFromBottom + (Vector2)transform.up * ObstacleHeightTolerance;
			_horizontalRayCastToTop = _horizontalRayCastToTop - (Vector2)transform.up * ObstacleHeightTolerance;

			// we determine the length of our rays
			float horizontalRayLength = Mathf.Abs(_speed.x * DeltaTime) + _boundsWidth / 2 + RayOffsetHorizontal * 2 + RayExtraLengthHorizontal;

			// we resize our storage if needed
			if (_sideHitsStorage.Length != NumberOfHorizontalRays)
			{
				_sideHitsStorage = new RaycastHit2D[NumberOfHorizontalRays];	
			}
                        
			// we cast rays to the sides
			for (int i = 0; i < NumberOfHorizontalRays;i++)
			{	
				Vector2 rayOriginPoint = Vector2.Lerp(_horizontalRayCastFromBottom, _horizontalRayCastToTop, (float)i / (float)(NumberOfHorizontalRays-1));

				// if we were grounded last frame and if this is our first ray, we don't cast against one way platforms
				if ( State.WasGroundedLastFrame && i == 0 )		
				{
					_sideHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,raysDirection*(transform.right),horizontalRayLength,PlatformMask, MMColors.Indigo,Parameters.DrawRaycastsGizmos);	
				}						
				else
				{
					_sideHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,raysDirection*(transform.right),horizontalRayLength,PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, MMColors.Indigo,Parameters.DrawRaycastsGizmos);			
				}
				// if we've hit something
				if (_sideHitsStorage[i].distance > 0)
				{	
					// if this collider is on our ignore list, we break
					if (_sideHitsStorage[i].collider == _ignoredCollider)
					{
						break;
					}
                    
					// we determine and store our current lateral slope angle
					float hitAngle = Mathf.Abs(Vector2.Angle(_sideHitsStorage[i].normal, transform.up));
                    
					if (OneWayPlatformMask.MMContains(_sideHitsStorage[i].collider.gameObject))
					{
						if (hitAngle > 90)
						{
							break;
						}
					}

					// we check if this is our movement direction
					if (_movementDirection == raysDirection)
					{
						State.LateralSlopeAngle = hitAngle;
					}                    

					// if the lateral slope angle is higher than our maximum slope angle, then we've hit a wall, and stop x movement accordingly
					if (hitAngle > Parameters.MaximumSlopeAngle)
					{
						if (raysDirection < 0)
						{
							State.IsCollidingLeft = true;
							State.DistanceToLeftCollider = _sideHitsStorage[i].distance;
						} 
						else
						{
							State.IsCollidingRight = true;
							State.DistanceToRightCollider = _sideHitsStorage[i].distance;
						}

						if ((_movementDirection == raysDirection) || (CastRaysOnBothSides && (_speed.x == 0f)))
						{
							CurrentWallCollider = _sideHitsStorage[i].collider.gameObject;
							State.SlopeAngleOK = false;

							float distance = MMMaths.DistanceBetweenPointAndLine(_sideHitsStorage[i].point, _horizontalRayCastFromBottom, _horizontalRayCastToTop);
							if (raysDirection <= 0)
							{
								_newPosition.x = -distance
								                 + _boundsWidth / 2
								                 + RayOffsetHorizontal * 2 ;
							}
							else
							{
								_newPosition.x = distance
								                 - _boundsWidth / 2
								                 - RayOffsetHorizontal * 2 ;
							}
                            
							// if we're in the air, we prevent the character from being pushed back.
							if (!State.IsGrounded && (Speed.y != 0) && (!Mathf.Approximately(hitAngle, 90f)))
							{
								_newPosition.x = 0;
							}

							_contactList.Add(_sideHitsStorage[i]);
							_speed.x = 0;
							_shouldComputeNewSpeed = true;
						}

						break;
					}
				}						
			}
		}

		/// <summary>
		/// Every frame, we cast a number of rays below our character to check for platform collisions
		/// </summary>
		protected virtual void CastRaysBelow()
		{
			_friction=0;

			if (_newPosition.y < -_smallValue)
			{
				State.IsFalling = true;
			}
			else
			{
				State.IsFalling = false;
			}

			if ((Parameters.Gravity > 0) && (!State.IsFalling))
			{
				State.IsCollidingBelow = false;
				return;
			}				

			float rayLength = (_boundsHeight / 2) + RayOffsetVertical + RayExtraLengthVertical; 	

			if (State.OnAMovingPlatform)
			{
				rayLength *= OnMovingPlatformRaycastLengthMultiplier;
			}	

			if (_newPosition.y < 0)
			{
				rayLength += Mathf.Abs(_newPosition.y);
			}			

			_verticalRayCastFromLeft = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
			_verticalRayCastToRight = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	
			_verticalRayCastFromLeft += (Vector2)transform.up * RayOffsetVertical;
			_verticalRayCastToRight += (Vector2)transform.up * RayOffsetVertical;
			_verticalRayCastFromLeft += (Vector2)transform.right * _newPosition.x;
			_verticalRayCastToRight += (Vector2)transform.right * _newPosition.x;

			if (_belowHitsStorage.Length != NumberOfVerticalRays)
			{
				_belowHitsStorage = new RaycastHit2D[NumberOfVerticalRays];	
			}

			_raysBelowLayerMaskPlatforms = PlatformMask;

			_raysBelowLayerMaskPlatformsWithoutOneWay = PlatformMask & ~MidHeightOneWayPlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask;
			_raysBelowLayerMaskPlatformsWithoutMidHeight = _raysBelowLayerMaskPlatforms & ~MidHeightOneWayPlatformMask;
            
			// if what we're standing on is a mid height oneway platform, we turn it into a regular platform for this frame only
			if (StandingOnLastFrame != null)
			{
				_savedBelowLayer = StandingOnLastFrame.layer;
				if (MidHeightOneWayPlatformMask.MMContains(StandingOnLastFrame.layer))
				{
					StandingOnLastFrame.layer = LayerMask.NameToLayer("Platforms");
				}
			}       
            
			// if we were grounded last frame, and not on a one way platform, we ignore any one way platform that would come in our path.
			if (State.WasGroundedLastFrame)
			{
				if (StandingOnLastFrame != null)
				{
					if (!MidHeightOneWayPlatformMask.MMContains(StandingOnLastFrame.layer))
					{
						_raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatformsWithoutMidHeight;                        
					}
				}
			}
            
			// stairs management
			if (State.WasGroundedLastFrame)
			{
				if (StandingOnLastFrame != null)
				{
					if (StairsMask.MMContains(StandingOnLastFrame.layer))
					{
						// if we're still within the bounds of the stairs
						if (StandingOnCollider.bounds.Contains(_colliderBottomCenterPosition))
						{
							_raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask | StairsMask;
						}    
					}
				}
			}

			if (State.OnAMovingPlatform && (_newPosition.y > 0))
			{
				_raysBelowLayerMaskPlatforms = _raysBelowLayerMaskPlatforms & ~OneWayPlatformMask;
			}

			float smallestDistance = float.MaxValue; 
			int smallestDistanceIndex = 0; 						
			bool hitConnected = false;
			StandingOn = null;
			for (int i = 0; i < NumberOfVerticalRays; i++)
			{
				StandingOnArray[i] = null;
			}
			
			for (int i = 0; i < NumberOfVerticalRays; i++)
			{
				Vector2 rayOriginPoint = Vector2.Lerp(_verticalRayCastFromLeft, _verticalRayCastToRight, (float)i / (float)(NumberOfVerticalRays - 1));
		
				if ((_newPosition.y > 0) && (!State.WasGroundedLastFrame))
				{
					_belowHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,-transform.up,rayLength, _raysBelowLayerMaskPlatformsWithoutOneWay, Color.blue,Parameters.DrawRaycastsGizmos);	
				}					
				else
				{
					_belowHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,-transform.up,rayLength, _raysBelowLayerMaskPlatforms, Color.blue,Parameters.DrawRaycastsGizmos);					
				}					

				float distance = MMMaths.DistanceBetweenPointAndLine (_belowHitsStorage [smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);	

				if (_belowHitsStorage[i])
				{
					if (_belowHitsStorage[i].collider == _ignoredCollider)
					{
						continue;
					}

					hitConnected = true;
					State.BelowSlopeAngle = Vector2.Angle( _belowHitsStorage[i].normal, transform.up );
					State.BlowSlopeNormal = _belowHitsStorage[i].normal;
					State.BelowSlopeAngleAbsolute = MMMaths.AngleBetween(_belowHitsStorage[i].normal, Vector2.up);
					
					StandingOnArray[i] = _belowHitsStorage[i].collider.gameObject;

					_crossBelowSlopeAngle = Vector3.Cross (transform.up, _belowHitsStorage [i].normal);
					if (_crossBelowSlopeAngle.z < 0)
					{
						State.BelowSlopeAngle = -State.BelowSlopeAngle;
					}

					if (_belowHitsStorage[i].distance < smallestDistance)
					{
						smallestDistanceIndex=i;
						smallestDistance = _belowHitsStorage[i].distance;
					}
				}

				if (distance < _smallValue)
				{
					break;
				}
			}
			if (hitConnected)
			{
				StandingOn = _belowHitsStorage[smallestDistanceIndex].collider.gameObject;
				StandingOnCollider = _belowHitsStorage [smallestDistanceIndex].collider;
                
				// if the character is jumping onto a (1-way) platform but not high enough, we do nothing
				if (
					!State.WasGroundedLastFrame
					&& (smallestDistance < _boundsHeight / 2) 
					&& (
						OneWayPlatformMask.MMContains(StandingOn.layer)
						||
						(MovingOneWayPlatformMask.MMContains(StandingOn.layer) && (_speed.y > 0))
					) 
				)
				{
						StandingOn = null;
						StandingOnCollider = null;
						State.IsCollidingBelow = false;
						return;
					
				}

				LastStandingOn = StandingOn;
				State.IsFalling = false;			
				State.IsCollidingBelow = true;


				// if we're applying an external force (jumping, jetpack...) we only apply that
				if (_externalForce.y > 0 && _speed.y > 0)
				{
					_newPosition.y = _speed.y * DeltaTime;
					State.IsCollidingBelow = false;
				}
				// if not, we just adjust the position based on the raycast hit
				else
				{
					float distance = MMMaths.DistanceBetweenPointAndLine (_belowHitsStorage [smallestDistanceIndex].point, _verticalRayCastFromLeft, _verticalRayCastToRight);

					_newPosition.y = -distance
					                 + _boundsHeight / 2 
					                 + RayOffsetVertical;
				}

				if (!State.WasGroundedLastFrame && _speed.y > 0)
				{
					_newPosition.y += _speed.y * DeltaTime;
				}				

				if (Mathf.Abs(_newPosition.y) < _smallValue)
				{
					_newPosition.y = 0;
				}					

				// we check if whatever we're standing on applies a friction change
				_frictionTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<SurfaceModifier>();
				if ((_frictionTest != null) && (_frictionTest.enabled))
				{
					_friction = _belowHitsStorage[smallestDistanceIndex].collider.GetComponent<SurfaceModifier>().Friction;
				}

				// we check if the character is standing on a moving platform
				_movingPlatformTest = _belowHitsStorage[smallestDistanceIndex].collider.gameObject.MMGetComponentNoAlloc<MMPathMovement>();
				if (_movingPlatformTest != null && State.IsGrounded)
				{
					_movingPlatform = _movingPlatformTest.GetComponent<MMPathMovement>();
				}
				else
				{
					DetachFromMovingPlatform();
				}
			}
			else
			{
				State.IsCollidingBelow=false;
				if(State.OnAMovingPlatform)
				{
					DetachFromMovingPlatform();
				}
			}	

			if (StickToSlopes)
			{
				StickToSlope();
			}
		}

		/// <summary>
		/// If we're in the air and moving up, we cast rays above the character's head to check for collisions
		/// </summary>
		protected virtual void CastRaysAbove()
		{
			float rayLength = State.IsGrounded ? RayOffsetVertical : _newPosition.y;
			rayLength += _boundsHeight / 2;

			bool hitConnected=false; 

			_aboveRayCastStart = (_boundsBottomLeftCorner + _boundsTopLeftCorner) / 2;
			_aboveRayCastEnd = (_boundsBottomRightCorner + _boundsTopRightCorner) / 2;	

			_aboveRayCastStart += (Vector2)transform.right * _newPosition.x;
			_aboveRayCastEnd += (Vector2)transform.right * _newPosition.x;

			if (_aboveHitsStorage.Length != NumberOfVerticalRays)
			{
				_aboveHitsStorage = new RaycastHit2D[NumberOfVerticalRays];	
			}

			float smallestDistance=float.MaxValue;

			int collidingIndex = 0;
			for (int i=0; i<NumberOfVerticalRays;i++)
			{							
				Vector2 rayOriginPoint = Vector2.Lerp(_aboveRayCastStart,_aboveRayCastEnd,(float)i/(float)(NumberOfVerticalRays-1));
				_aboveHitsStorage[i] = MMDebug.RayCast (rayOriginPoint,(transform.up), rayLength, PlatformMask & ~OneWayPlatformMask & ~MovingOneWayPlatformMask, MMColors.Cyan, Parameters.DrawRaycastsGizmos);	

				if (_aboveHitsStorage[i])
				{
					hitConnected=true;
					collidingIndex = i;

					if (_aboveHitsStorage[i].collider == _ignoredCollider)
					{
						break;
					}
					if (_aboveHitsStorage[i].distance<smallestDistance)
					{
						smallestDistance = _aboveHitsStorage[i].distance;
					}
				}					
			}	

			if (hitConnected)
			{
				_newPosition.y = smallestDistance - _boundsHeight / 2;
                
				if ((State.IsGrounded) && (_newPosition.y < 0))
				{
					_newPosition.y = 0;
				}

				State.IsCollidingAbove = true;

				if (!State.WasTouchingTheCeilingLastFrame)
				{
					_speed = new Vector2(_speed.x, 0f);
				}

				SetVerticalForce(0);
			}	
		}

		/// <summary>
		/// If we're going down a slope, sticks the character to the slope to avoid bounces
		/// </summary>
		protected virtual void StickToSlope()
		{
			// if we're in the air, don't have to stick to slopes, being pushed up or on a moving platform, we exit
			if ((_newPosition.y >= StickToSlopesOffsetY)
			    || (_newPosition.y <= -StickToSlopesOffsetY)
			    || (State.IsJumping)
			    || (!StickToSlopes)
			    || (!State.WasGroundedLastFrame)
			    || (_externalForce.y > 0)
			    || (_movingPlatform != null))
			{
				// edge case for stairs
				if (!(!State.WasGroundedLastFrame
				      && ((LastStandingOn != null) && StairsMask.MMContains(LastStandingOn.layer))
				      && !(State.IsJumping) 
				    ))
				{
					return;
				}
			}

			if ((_characterGravity != null) && (_characterGravity.InGravityPointRange))
			{
				return;
			}

			// we determine our ray's length
			float rayLength = 0;
			if (StickyRaycastLength == 0)
			{
				rayLength = _boundsWidth * Mathf.Abs(Mathf.Tan(Parameters.MaximumSlopeAngle * Mathf.Deg2Rad));
				rayLength += _boundsHeight / 2 + RayOffsetVertical;
			}
			else
			{
				rayLength = StickyRaycastLength;
			}

			// we cast rays on both sides to know what we're standing on
			_rayCastOrigin.y = _boundsCenter.y;

			_rayCastOrigin.x = _boundsBottomLeftCorner.x;
			_rayCastOrigin.x += _newPosition.x;
			_stickRaycastLeft = MMDebug.RayCast(_rayCastOrigin, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, MMColors.LightBlue, Parameters.DrawRaycastsGizmos);

			_rayCastOrigin.x = _boundsBottomRightCorner.x;
			_rayCastOrigin.x += _newPosition.x;
			_stickRaycastRight = MMDebug.RayCast(_rayCastOrigin, -transform.up, rayLength, _raysBelowLayerMaskPlatforms, MMColors.LightBlue, Parameters.DrawRaycastsGizmos);

			bool castFromLeft = false;
			float belowSlopeAngleLeft = Vector2.Angle(_stickRaycastLeft.normal, transform.up);
			Vector3 crossBelowSlopeAngleLeft = Vector3.Cross(transform.up, _stickRaycastLeft.normal);
			if (crossBelowSlopeAngleLeft.z < 0)
			{
				belowSlopeAngleLeft = -belowSlopeAngleLeft;
			}

			float belowSlopeAngleRight = Vector2.Angle(_stickRaycastRight.normal, transform.up);
			Vector3 crossBelowSlopeAngleRight = Vector3.Cross(transform.up, _stickRaycastRight.normal);
			if (crossBelowSlopeAngleRight.z < 0)
			{
				belowSlopeAngleRight = -belowSlopeAngleRight;
			}
            
			float belowSlopeAngle = 0f;
            
            
			castFromLeft = (Mathf.Abs(belowSlopeAngleLeft) > Mathf.Abs(belowSlopeAngleRight));

			// if we're on a slope
			if (belowSlopeAngleLeft == belowSlopeAngleRight)
			{
				belowSlopeAngle = belowSlopeAngleLeft;
				castFromLeft = (belowSlopeAngle < 0f);
			}

			// if we have a slope on the right and flat on the left
			if ((belowSlopeAngleLeft == 0f) && (belowSlopeAngleRight != 0f))
			{
				belowSlopeAngle = belowSlopeAngleLeft;
				castFromLeft = (belowSlopeAngleRight < 0f);
			}

			// if we have flat on the right and a slope on the left
			if ((belowSlopeAngleLeft != 0f) && (belowSlopeAngleRight == 0f))
			{
				belowSlopeAngle = belowSlopeAngleRight;
				castFromLeft = (belowSlopeAngleLeft < 0f);
			}

			// if both angles aren't flat
			if ((belowSlopeAngleLeft != 0f) && (belowSlopeAngleRight != 0f))
			{
				castFromLeft = (_stickRaycastLeft.distance < _stickRaycastRight.distance);
				belowSlopeAngle = (castFromLeft) ? belowSlopeAngleLeft : belowSlopeAngleRight;
			}     
            
			// if we're on a damn spike, we handle it and exit
			if ((belowSlopeAngleLeft > 0f) && (belowSlopeAngleRight < 0f))
			{
				_stickRaycast = MMDebug.BoxCast(_boundsCenter, Bounds, Vector2.Angle(transform.up, Vector2.up), -transform.up, rayLength, _raysBelowLayerMaskPlatforms, Color.red, true);
				if (_stickRaycast)
				{
					if (_stickRaycast.collider == _ignoredCollider)
					{
						return;
					}

					_newPosition.y = -Mathf.Abs(_stickRaycast.point.y - _rayCastOrigin.y)
					                 + _boundsHeight / 2;

					State.IsCollidingBelow = true;
				}

				return;
			}

			_stickRaycast = castFromLeft ? _stickRaycastLeft : _stickRaycastRight;	

			// we cast a ray, if it hits, we move to match its height
			if (_stickRaycast)
			{
				if (_stickRaycast.collider == _ignoredCollider)
				{
					return;
				}

				_newPosition.y = -Mathf.Abs(_stickRaycast.point.y - _rayCastOrigin.y) 
				                 + _boundsHeight / 2 ;

				State.IsCollidingBelow = true;
			}	
		}

		/// <summary>
		/// Computes the new speed of the character
		/// </summary>
		protected virtual void ComputeNewSpeed()
		{
			// we compute the new speed
			if ((DeltaTime > 0) && _shouldComputeNewSpeed)
			{
				_speed = _newPosition / DeltaTime;	
			}	

			// we apply our slope speed factor based on the slope's angle
			if (State.IsGrounded)
			{
				_speed.x *= Parameters.SlopeAngleSpeedFactor.Evaluate(Mathf.Abs(State.BelowSlopeAngle) * Mathf.Sign(_speed.y));
			}

			if (!State.OnAMovingPlatform)				
			{
				// we make sure the velocity doesn't exceed the MaxVelocity specified in the parameters
				ClampSpeed();
				ClampExternalForce();
			}
		}

		protected virtual void ClampSpeed()
		{
			_speed.x = Mathf.Clamp(_speed.x,-Parameters.MaxVelocity.x,Parameters.MaxVelocity.x);
			_speed.y = Mathf.Clamp(_speed.y,-Parameters.MaxVelocity.y,Parameters.MaxVelocity.y);
		}

		protected virtual void ClampExternalForce()
		{
			_externalForce.x = Mathf.Clamp(_externalForce.x,-Parameters.MaxVelocity.x,Parameters.MaxVelocity.x);
			_externalForce.y = Mathf.Clamp(_externalForce.y,-Parameters.MaxVelocity.y,Parameters.MaxVelocity.y);
		}

		/// <summary>
		/// Sets grounded state and collision states
		/// </summary>
		protected virtual void SetStates()
		{
			// we change states depending on the outcome of the movement
			if( !State.WasGroundedLastFrame && State.IsCollidingBelow )
			{
				State.JustGotGrounded=true;
			}

			if (State.IsCollidingLeft || State.IsCollidingRight || State.IsCollidingBelow || State.IsCollidingAbove)
			{
				OnCorgiColliderHit();
			}	
		}

		/// <summary>
		/// Computes the distance to the ground
		/// </summary>
		protected virtual void ComputeDistanceToTheGround()
		{
			if (DistanceToTheGroundRayMaximumLength <= 0)
			{
				return;
			}

			if (State.IsGrounded)
			{
				_distanceToTheGround = 0f;
				return;
			}

			_rayCastOrigin.x = (State.BelowSlopeAngle < 0) ? _boundsBottomLeftCorner.x : _boundsBottomRightCorner.x;
			_rayCastOrigin.y = _boundsCenter.y;

			_distanceToTheGroundRaycast = MMDebug.RayCast(_rayCastOrigin, -transform.up, DistanceToTheGroundRayMaximumLength, _raysBelowLayerMaskPlatforms, MMColors.CadetBlue, true);

			if (_distanceToTheGroundRaycast)
			{
				if (_distanceToTheGroundRaycast.collider == _ignoredCollider)
				{
					_distanceToTheGround = -1f;
					return;
				}
				_distanceToTheGround = _distanceToTheGroundRaycast.distance - _boundsHeight / 2 ;
			}
			else
			{
				_distanceToTheGround = -1f;
			}
		}

		/// <summary>
		/// Creates a rectangle with the boxcollider's size for ease of use and draws debug lines along the different raycast origin axis
		/// </summary>
		public virtual void SetRaysParameters() 
		{		
			float top = _boxCollider.offset.y + (_boxCollider.size.y / 2f);
			float bottom = _boxCollider.offset.y - (_boxCollider.size.y / 2f);
			float left = _boxCollider.offset.x - (_boxCollider.size.x / 2f);
			float right = _boxCollider.offset.x + (_boxCollider.size.x /2f);

			_boundsTopLeftCorner.x = left;
			_boundsTopLeftCorner.y = top;

			_boundsTopRightCorner.x = right;
			_boundsTopRightCorner.y = top;

			_boundsBottomLeftCorner.x = left;
			_boundsBottomLeftCorner.y = bottom;

			_boundsBottomRightCorner.x = right;
			_boundsBottomRightCorner.y = bottom;

			_boundsTopLeftCorner = transform.TransformPoint (_boundsTopLeftCorner);
			_boundsTopRightCorner = transform.TransformPoint (_boundsTopRightCorner);
			_boundsBottomLeftCorner = transform.TransformPoint (_boundsBottomLeftCorner);
			_boundsBottomRightCorner = transform.TransformPoint (_boundsBottomRightCorner);
			_boundsCenter = _boxCollider.bounds.center;

			_boundsWidth = Vector2.Distance (_boundsBottomLeftCorner, _boundsBottomRightCorner);
			_boundsHeight = Vector2.Distance (_boundsBottomLeftCorner, _boundsTopLeftCorner);
		}

		public virtual void SetIgnoreCollider(Collider2D newIgnoredCollider)
		{
			_ignoredCollider = newIgnoredCollider;
		}

		/// <summary>
		/// Disables the collisions for the specified duration
		/// </summary>
		/// <param name="duration">the duration for which the collisions must be disabled</param>
		public virtual IEnumerator DisableCollisions(float duration)
		{
			// we turn the collisions off
			CollisionsOff();
			// we wait for a few seconds
			yield return new WaitForSeconds (duration);
			// we turn them on again
			CollisionsOn();
		}

		/// <summary>
		/// Resets the collision mask with the default settings
		/// </summary>
		public virtual void CollisionsOn()
		{
			PlatformMask = _platformMaskSave;
			PlatformMask |= OneWayPlatformMask;
			PlatformMask |= MovingPlatformMask;
			PlatformMask |= MovingOneWayPlatformMask;
			PlatformMask |= MidHeightOneWayPlatformMask;
		}

		/// <summary>
		/// Turns all collisions off
		/// </summary>
		public virtual void CollisionsOff()
		{
			PlatformMask=0;
		}

		protected GameObject _detachmentObject;

		/// <summary>
		/// Disables the collisions with one way platforms for the specified duration
		/// </summary>
		/// <param name="duration">the duration for which the collisions must be disabled</param>
		public virtual IEnumerator DisableCollisionsWithOneWayPlatforms(float duration)
		{
			switch (DetachmentMethod)
			{
				case DetachmentMethods.Layer:
					// we make it fall down below the platform by moving it just below the platform
					this.transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
					// we turn the collisions off
					CollisionsOffWithOneWayPlatformsLayer();
					// we wait for a few seconds
					yield return new WaitForSeconds(duration);
					// we turn them on again
					CollisionsOn();
					break;
				case DetachmentMethods.Object:
					// we set our current platform collider as ignored
					SetIgnoreCollider(StandingOnCollider);
					// we wait for a few seconds
					yield return new WaitForSeconds(duration);
					// we turn clear it
					SetIgnoreCollider(null);
					break;
				case DetachmentMethods.LayerChange:
					_detachmentObject = StandingOnCollider.gameObject;
					int saveLayer = _detachmentObject.layer;
					_detachmentObject.layer = DetachmentLayer.LayerIndex;
					yield return new WaitForSeconds(duration);
					_detachmentObject.layer = saveLayer;
					break;
			}
		}

		/// <summary>
		/// Disables the collisions with moving platforms for the specified duration
		/// </summary>
		/// <param name="duration">the duration for which the collisions must be disabled</param>
		public virtual IEnumerator DisableCollisionsWithMovingPlatforms(float duration)
		{
			switch (DetachmentMethod)
			{
				case DetachmentMethods.Layer:
					CollisionsOffWithMovingPlatformsLayer ();
					yield return new WaitForSeconds (duration);
					CollisionsOn ();
					break;
				case DetachmentMethods.Object:
					// we set our current platform collider as ignored
					SetIgnoreCollider (StandingOnCollider);
					// we wait for a few seconds
					yield return new WaitForSeconds (duration);
					// we turn clear it
					SetIgnoreCollider (null);	
					break;
				case DetachmentMethods.LayerChange:
					_detachmentObject = StandingOnCollider.gameObject;
					int saveLayer = _detachmentObject.layer;
					_detachmentObject.layer = DetachmentLayer.LayerIndex;
					yield return new WaitForSeconds(duration);
					_detachmentObject.layer = saveLayer;
					break;
			}
		}

		/// <summary>
		/// Disables collisions only with the one way platform layers
		/// </summary>
		public virtual void CollisionsOffWithOneWayPlatformsLayer()
		{
			PlatformMask -= OneWayPlatformMask;
			PlatformMask -= MovingOneWayPlatformMask;
			PlatformMask -= MidHeightOneWayPlatformMask;
		}

		/// <summary>
		/// Disables collisions only with moving platform layers
		/// </summary>
		public virtual void CollisionsOffWithMovingPlatformsLayer()
		{
			PlatformMask -= MovingPlatformMask;
			PlatformMask -= MovingOneWayPlatformMask;
		}

		/// <summary>
		/// Enables collisions with the stairs layer
		/// </summary>
		public virtual void CollisionsOnWithStairs()
		{
			if (!_collisionsOnWithStairs)
			{
				PlatformMask = PlatformMask | StairsMask;
				OneWayPlatformMask = OneWayPlatformMask | StairsMask;
				_collisionsOnWithStairs = true;
				CollisionsOn();
			}
		}

		/// <summary>
		/// Disables collisions with the stairs layer
		/// </summary>
		public virtual void CollisionsOffWithStairs()
		{
			if (_collisionsOnWithStairs)
			{
				PlatformMask = PlatformMask - StairsMask;
				OneWayPlatformMask = OneWayPlatformMask - StairsMask;
				_collisionsOnWithStairs = false;
			}
		}

		/// <summary>
		/// Resets all overridden parameters.
		/// </summary>
		public virtual void ResetParameters()
		{
			_overrideParameters = DefaultParameters;
		}

		/// <summary>
		/// Slows the character's fall by the specified factor.
		/// </summary>
		/// <param name="factor">Factor.</param>
		public virtual void SlowFall(float factor)
		{
			_fallSlowFactor = factor;
		}

		/// <summary>
		/// Activates or desactivates the gravity for this character only.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, activates the gravity. If set to <c>false</c>, turns it off.</param>	   
		public virtual void GravityActive(bool state)
		{
			if (state)
			{
				_gravityActive = true;
			}
			else
			{
				_gravityActive = false;
			}
		}

		/// <summary>
		/// Resizes the collider to the new size set in parameters
		/// This is temporary - typically when crouching - and will be reverted by ResetColliderSize()
		/// </summary>
		/// <param name="newSize">New size.</param>
		public virtual void ResizeCollider(Vector2 newSize) 
		{
			float newYOffset = _originalColliderOffset.y - (_originalColliderSize.y - newSize.y) / 2 ;

			_boxCollider.size = newSize;
			_boxCollider.offset = newYOffset * Vector3.up;
			SetRaysParameters();
			State.ColliderResized = true;
		}

		/// <summary>
		/// Resizes the collider to the new size set in parameters, won't be reverted by ResetColliderSize()
		/// </summary>
		/// <param name="newSize"></param>
		public virtual void ResizeColliderPermanently(Vector2 newSize)
		{
			ResizeCollider(newSize);
			_originalColliderSize = _boxCollider.size;
			_originalColliderOffset = _boxCollider.offset;
			SetRaysParameters();
			State.ColliderResized = false;
		}

		/// <summary>
		/// Returns the collider to its initial size
		/// </summary>
		public virtual void ResetColliderSize()
		{
			_boxCollider.size = _originalColliderSize;
			_boxCollider.offset = _originalColliderOffset;
			SetRaysParameters();
			State.ColliderResized = false;
		}

		/// <summary>
		/// Determines whether this instance can go back to original size.
		/// </summary>
		/// <returns><c>true</c> if this instance can go back to original size; otherwise, <c>false</c>.</returns>
		public virtual bool CanGoBackToOriginalSize()
		{
			// if we're already at original size, we return true
			if (_boxCollider.size == _originalColliderSize)
			{
				return true;
			}
			float headCheckDistance = _originalColliderSize.y * transform.localScale.y * CrouchedRaycastLengthMultiplier;

			// we cast two rays above our character to check for obstacles. If we didn't hit anything, we can go back to original size, otherwise we can't
			_originalSizeRaycastOrigin = _boundsBottomLeftCorner + (Vector2)transform.up * _smallValue;
			bool headCheckLeft = MMDebug.RayCast(_originalSizeRaycastOrigin, transform.up, headCheckDistance, PlatformMask - OneWayPlatformMask, MMColors.LightSlateGray, true);

			_originalSizeRaycastOrigin = _boundsBottomRightCorner + (Vector2)transform.up * _smallValue;
			bool headCheckRight = MMDebug.RayCast(_originalSizeRaycastOrigin, transform.up, headCheckDistance, PlatformMask - OneWayPlatformMask, MMColors.LightSlateGray, true);
			if (headCheckLeft || headCheckRight)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Tries to move to a safe position, and moves to the closest position if specified. Returns true if movement was possible, false otherwise 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="moveToClosestPosition"></param>
		/// <returns></returns>
		public virtual bool TrySetSafeTransformPosition(Vector3 position, bool moveToClosestPosition = false)
		{
			Vector3 safestPosition = GetClosestSafePosition(position);
			if (safestPosition != position)
			{
				if (moveToClosestPosition)
				{
					this.transform.position = safestPosition;
					Physics2D.SyncTransforms();
					return true;
				}
				return false;
			}
			else
			{
				this.transform.position = position;
				Physics2D.SyncTransforms();
				return true;
			}
		}

		/// <summary>
		/// Moves the controller's transform to the desired position
		/// </summary>
		/// <param name="position"></param>
		public virtual void SetTransformPosition(Vector3 position)
		{
			if (SafeSetTransform)
			{
				this.transform.position = GetClosestSafePosition(position);
			}
			else
			{
				this.transform.position = position;
			}
			Physics2D.SyncTransforms();
		}

		/// <summary>
		/// Returns the closest "safe" point (not overlapping any platform) to the destination
		/// </summary>
		/// <param name="destination"></param>
		/// <returns></returns>
		public virtual Vector2 GetClosestSafePosition(Vector2 destination)
		{
			// we do a first test to see if there's room enough to move to the destination
			Collider2D hit = Physics2D.OverlapBox(destination, _boxCollider.size, this.transform.rotation.eulerAngles.z, PlatformMask);

			if (hit == null)
			{
				return destination;
			}                
			else
			{
				// if the original destination wasn't safe, we find the closest safe point between our controller and the obstacle
				destination -= RayOffsetHorizontal * (Vector2)(hit.transform.position - this.transform.position).normalized;
				hit = Physics2D.OverlapBox(destination, _boxCollider.size, this.transform.rotation.eulerAngles.z, PlatformMask);

				if (hit == null)
				{
					return destination;
				}
				else
				{
					return this.transform.position;
				}
			}
		}

		/// <summary>
		/// Teleports the character to the ground
		/// </summary>
		public virtual void AnchorToGround()
		{
			ComputeDistanceToTheGround();
			if (_distanceToTheGround > 0f)
			{
				Vector3 newPosition;
				newPosition.x = this.transform.position.x;
				newPosition.y = this.transform.position.y - _distanceToTheGround;
				newPosition.z = this._transform.position.z;
				SetTransformPosition(newPosition);
		        
				State.IsFalling = false;			
				State.IsCollidingBelow = true;
				_speed = Vector2.zero;
				_externalForce = Vector2.zero;
			}
		}

		/// <summary>
		/// triggered when the character's raycasts collide with something 
		/// </summary>
		protected virtual void OnCorgiColliderHit() 
		{
			foreach (RaycastHit2D hit in _contactList )
			{	
				if (Parameters.Physics2DInteraction)
				{
					Rigidbody2D body = hit.collider.attachedRigidbody;
					if (body == null || body.isKinematic || body.bodyType == RigidbodyType2D.Static)
					{
						return;
					}                        
					Vector3 pushDirection = new Vector3(_externalForce.x, 0, 0);
					body.velocity = pushDirection.normalized * Parameters.Physics2DPushForce;		
				}	
			}	
		}

		/// <summary>
		/// triggered when the character enters a collider
		/// </summary>
		/// <param name="collider">the object we're colliding with.</param> 
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			CorgiControllerPhysicsVolume2D parameters = collider.gameObject.MMGetComponentNoAlloc<CorgiControllerPhysicsVolume2D>();
			if (parameters != null)
			{
				// if the object we're colliding with has parameters, we apply them to our character.
				_overrideParameters = parameters.ControllerParameters;	
				if (parameters.ResetForcesOnEntry)
				{
					SetForce (Vector2.zero);
				}
				if (parameters.MultiplyForcesOnEntry)
				{
					SetForce(Vector2.Scale(parameters.ForceMultiplierOnEntry,Speed));
				}
			}
		}

		/// <summary>
		/// triggered while the character stays inside another collider
		/// </summary>
		/// <param name="collider">the object we're colliding with.</param>
		protected virtual void OnTriggerStay2D( Collider2D collider )
		{
		}

		/// <summary>
		/// triggered when the character exits a collider
		/// </summary>
		/// <param name="collider">the object we're colliding with.</param>
		protected virtual void OnTriggerExit2D(Collider2D collider)
		{		
			CorgiControllerPhysicsVolume2D parameters = collider.gameObject.MMGetComponentNoAlloc<CorgiControllerPhysicsVolume2D>();
			if (parameters != null)
			{
				// if the object we were colliding with had parameters, we reset our character's parameters
				_overrideParameters = null;	
			}
		}
	}
}
