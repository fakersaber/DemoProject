/*************************************************************
 *           Unity Object Pool (c) by ClockStone 2017        *
 * 
 * Allows to "pool" prefab objects to avoid large number of
 * Instantiate() calls.
 * 
 * Usage:
 * 
 * Add the PoolableObject script component to the prefab to be pooled.
 * You can set the maximum number of objects to be be stored in the 
 * pool from within the inspector.
 * 
 * Replace all Instantiate( myPrefab ) calls with 
 * ObjectPoolController.Instantiate( myPrefab)
 * 
 * Replace all Destroy( myObjectInstance ) calls with 
 * ObjectPoolController.Destroy( myObjectInstance )
 * 
 * Replace all DestroyImmediate( myObjectInstance ) calls with 
 * ObjectPoolController.DestroyImmediate( myObjectInstance )
 * 
 * Note that Awake(), and OnDestroy() get called correctly for 
 * pooled objects. However, make sure that all component data that could  
 * possibly get changed during its lifetime get reinitialized by the
 * Awake() function.
 * The Start() function gets also called, but just after the Awake() function
 * during ObjectPoolController.Instantiate(...)
 * 
 * If a poolable objects gets parented to none-poolable object, the parent must
 * be destroyed using ObjectPoolController.Destroy( ... )
 * 
 * Be aware that OnDestroy() will get called multiple times: 
 *   a) the time ObjectPoolController.Destroy() is called when the object is added
 *      to the pool
 *   b) when the object really gets destroyed (e.g. if a new scene is loaded)
 *   
 * References to pooled objects will not change to null anymore once an object has 
 * been "destroyed" and moved to the pool. Use PoolableReference if you need such checks.
 * 
 * ********************************************************************
*/

#if UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0
#define UNITY_3_x
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using MessengerExtensions;
using System;

#pragma warning disable 1591 // undocumented XML code warning

namespace ClockStone
{
    internal static class PoolableExtensions
    {
        internal static void _SetActive( this GameObject obj, bool active )
        {
#if UNITY_3_x
            if ( active )
            {
                obj.SetActiveRecursively( true );
            } else 
                obj.active = false;
#else
            obj.SetActive( active );
#endif
        }

        internal static bool _GetActive( this GameObject obj )
        {
#if UNITY_3_x
            return gameObject.active;            
#else
            return obj.activeInHierarchy;
#endif
        }
    }

    /// <summary>
    /// Add this component to your prefab to make it poolable. 
    /// </summary>
    /// <remarks>
    /// See <see cref="ObjectPoolController"/> for an explanation how to set up a prefab for pooling.
    /// The following messages are sent to a poolable object:
    /// <list type="bullet">
    /// <item> 
    ///   <c>Awake()</c>, <c>Start()</c> and <c>OnDestroy()</c> whenever a poolable object is activated 
    ///   or deactivated from the pool (when the prefab used to instantiate is active itself). 
    ///   This way the same behaviour is simulated as if the object was instantiated respectively destroyed.
    ///   These messages are only sent when <see cref="sendAwakeStartOnDestroyMessage"/> is enabled.
    /// </item>
    /// <item>
    ///   <c>OnPoolableObjectActivated()</c> and <c>OnPoolableObjectDeactivated()</c> whenever a poolable 
    ///   object is activated or deactivated from the pool.
    ///   These messages are only sent when <see cref="sendPoolableActivateDeactivateMessages"/> is enabled.
    /// </item>
    /// </list>
    /// </remarks>
    /// <seealso cref="ObjectPoolController"/>
    [AddComponentMenu( "ClockStone/PoolableObject" )]
    public class PoolableObject : MonoBehaviour
    {
        /// <summary>
        /// The maximum number of instances of this prefab to get stored in the pool.
        /// </summary>
        public int maxPoolSize = 10;

        /// <summary>
        /// This number of instances will be preloaded to the pool if <see cref="ObjectPoolController.Preload(GameObject)"/> is called.
        /// </summary>
        public int preloadCount = 0;

        /// <summary>
        /// If enabled the object will not get destroyed if a new scene is loaded
        /// </summary>
        public bool doNotDestroyOnLoad = false;

        /// <summary>
        /// If enabled Awake(), Start(), and OnDestroy() messages are sent to the poolable object if the object is set 
        /// active respectively inactive whenever <see cref="ObjectPoolController.Destroy(GameObject)"/> or 
        /// <see cref="ObjectPoolController.Instantiate(GameObject)"/> is called. <para/>
        /// This way it is simulated that the object really gets instantiated respectively destroyed.
        /// </summary>
        /// <remarks>
        /// The Start() function is called immedialtely after Awake() by <see cref="ObjectPoolController.Instantiate(GameObject)"/> 
        /// and not next frame. So do not set data after <see cref="ObjectPoolController.Instantiate(GameObject)"/> that Start()
        /// relies on. In some cases you may not want the  Awake(), Start(), and OnDestroy() messages to be sent for performance 
        /// reasons because it may not be necessary to fully reinitialize a game object each time it is activated from the pool. 
        /// You can still use the <c>OnPoolableObjectActivated</c> and <c>OnPoolableObjectDeactivated</c> messages to initialize 
        /// specific data.
        /// The Awake() and Start() Messages only get sent when the instantiated object got instantiated from an active prefab
        /// parent. If the prefab was deactivated then the instantiated object will also be inactive and the Methods aren't called. 
        /// </remarks>
        public bool sendAwakeStartOnDestroyMessage = true;

        /// <summary>
        /// If enabled a <c>OnPoolableObjectActivated</c> and <c>OnPoolableObjectDeactivated</c> message is sent to 
        /// the poolable instance if the object is activated respectively deactivated by the <see cref="ObjectPoolController"/>
        /// </summary>
        public bool sendPoolableActivateDeactivateMessages = false;

        /// <summary>
        /// If enabled reflection gets used to invoke the <c>Awake()</c>, <c>Start()</c>, <c>OnDestroy()</c>, 
        /// <c>OnPoolableObjectActivated()</c> and <c>OnPoolableObjectDeactivated()</c> Methods instead of using the Unity-
        /// Messaging-System. This is useful when objects are instantiated as inactive or deactivated before they are destroyed.
        /// (Unity-Messaging-System works on active components and GameObjects only!)
        /// </summary>
        /// <remarks>
        /// * Invocations when an object gets instantiated (taken from pool):
        ///   - <c>Awake()</c> on active Components
        ///   - <c>Start()</c> on active Components
        ///   - <c>OnPoolableObjectActivated()</c> on all Components (also inactive)
        ///     (when an object is instantiated as inactive <c>Awake()</c> and <c>Start()</c> are never called)
        /// 
        /// * Invocations when an object gets destroyed (moved to pool):
        ///   - <c>OnPoolableObjectDeactivated()</c> on all Components (also inactive)
        ///   - <c>OnDestroy()</c> on all Components (also inactive)
        /// </remarks>
        public bool useReflectionInsteadOfMessages = false;

        internal bool _isInPool = false;

        /// <summary>
        /// if null - Object was not created from ObjectPoolController
        /// </summary>
        internal ObjectPoolController.ObjectPool _pool = null;

        internal int _serialNumber = 0;
        internal int _usageCount = 0;

        //needed when an object gets instantiated deactivated to prevent double awake
        internal bool _awakeJustCalledByUnity = false;
        internal bool _instantiatedByObjectPoolController = false;

        private bool _justInvokingOnDestroy = false;

        protected void Awake()
        {
            _awakeJustCalledByUnity = true;

#if UNITY_EDITOR
            if ( _pool == null && !ObjectPoolController._isDuringInstantiate && !_instantiatedByObjectPoolController )
                Debug.LogWarning( "Poolable object " + name + " was instantiated without ObjectPoolController" );
#endif

        }

        protected void OnDestroy()
        {
            //only if destroy message comes from unity and not from invocation
            if ( !_justInvokingOnDestroy && _pool != null )
            {
                // Poolable object was destroyed by using the default Unity Destroy() function -> Use ObjectPoolController.Destroy() instead
                // This can also happen if objects are automatically deleted by Unity e.g. due to level change or if an object is parented to an object that gets destroyed
                _pool.Remove( this );
            }
        }

        /// <summary>
        /// Gets the object's pool serial number. Each object has a unique serial number. Can be useful for debugging purposes.
        /// </summary>
        /// <returns>
        /// The serial number (starting with 1 for each pool).
        /// </returns>
        public int GetSerialNumber() // each new instance receives a unique serial number
        {
            return _serialNumber;
        }

        /// <summary>
        /// Gets the usage counter which gets increased each time an object is re-used from the pool.
        /// </summary>
        /// <returns>
        /// The usage counter
        /// </returns>
        public int GetUsageCount()
        {
            return _usageCount;
        }

        /// <summary>
        /// Moves all poolable objects of this kind (instantiated from the same prefab as this instance) back to the pool. 
        /// </summary>
        /// <returns>
        /// The number of instances deactivated and moved back to its pool.
        /// </returns>
        public int DeactivateAllPoolableObjectsOfMyKind()
        {
            if ( _pool != null )
            {
                return _pool._SetAllAvailable();
            }
            return 0;
        }

        /// <summary>
        /// Checks if the object is deactivated and in the pool.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the object is in the pool of deactivated objects, otherwise <c>false</c>.
        /// </returns>
        public bool IsDeactivated()
        {
            return _isInPool;
        }

        internal void _PutIntoPool()
        {
            if ( _pool == null )
            {
                Debug.LogError( "Tried to put object into pool which was not created with ObjectPoolController", this );
                return;
            }

            if ( _isInPool )
            {
                if ( transform.parent != _pool.poolParent )
                {
                    Debug.LogWarning( "Object was already in pool but parented to Pool-Parent. Reparented.", this );
                    transform.parent = _pool.poolParent;
                    return;
                }

                Debug.LogWarning( "Object is already in Pool", this );
                return;
            }

            //dont fire callbacks when object is put into pool initially
            if ( !ObjectPoolController._isDuringInstantiate )
            {
                if ( sendAwakeStartOnDestroyMessage )
                {
                    _justInvokingOnDestroy = true;
                    _pool.CallMethodOnObject( gameObject, "OnDestroy", true, true, useReflectionInsteadOfMessages );
                    _justInvokingOnDestroy = false;
                }

                if ( sendPoolableActivateDeactivateMessages )
                    _pool.CallMethodOnObject( gameObject, "OnPoolableObjectDeactivated", true, true, useReflectionInsteadOfMessages );
            }

            _isInPool = true;
            transform.parent = _pool.poolParent;

            gameObject.SetActive( false );
        }

        internal void TakeFromPool( Transform parent, bool activateObject )
        {
            if ( !_isInPool )
            {
                Debug.LogError( "Tried to take an object from Pool which is not available!", this );
                return;
            }

            _isInPool = false;

            _usageCount++;
            transform.parent = parent;

            if ( activateObject )
            {
                //this may be set to true when unity calls Awake after gameObject.SetActive(true);
                _awakeJustCalledByUnity = false;
                gameObject.SetActive( true );

                if ( sendAwakeStartOnDestroyMessage )
                {
                    //when an instance gets activated not the first time Awake() wont be called again. so we call it here via reflection!
                    if ( !_awakeJustCalledByUnity )
                    {
                        _pool.CallMethodOnObject( gameObject, "Awake", true, false, useReflectionInsteadOfMessages );

                        if ( gameObject._GetActive() ) // Awake could deactivate object
                            _pool.CallMethodOnObject( gameObject, "Start", true, false, useReflectionInsteadOfMessages );
                    }
                }

                if ( sendPoolableActivateDeactivateMessages )
                {
                    _pool.CallMethodOnObject( gameObject, "OnPoolableObjectActivated", true, true, useReflectionInsteadOfMessages );
                }
            }
        }
    }

    /// <summary>
    /// A static class used to create and destroy poolable objects.
    /// </summary>
    /// <remarks>
    /// What is pooling? <para/>
    /// GameObject.Instantiate(...) calls are relatively time expensive. If objects of the same
    /// type are frequently created and destroyed it is good practice to use object pools, particularly on mobile
    /// devices. This can greatly reduce the performance impact for object creation and garbage collection. <para/>
    /// How does pooling work?<para/>
    /// Instead of actually destroying object instances, they are just set inactive and moved to an object "pool".
    /// If a new object is requested it can then simply be pulled from the pool, instead of creating a new instance. <para/>
    /// Awake(), Start() and OnDestroy() are called if objects are retrieved from or moved to the pool like they 
    /// were instantiated and destroyed normally.
    /// </remarks>
    /// <example>
    /// How to set up a prefab for pooling:
    /// <list type="number">
    /// <item>Add the PoolableObject script component to the prefab to be pooled.
    /// You can set the maximum number of objects to be be stored in the pool from within the inspector.</item>
    /// <item> Replace all <c>Instantiate( myPrefab )</c> calls with <c>ObjectPoolController.Instantiate( myPrefab )</c></item>
    /// <item> Replace all <c>Destroy( myObjectInstance )</c> calls with <c>ObjectPoolController.Destroy( myObjectInstance )</c></item>
    /// </list>
    /// Attention: Be aware that:
    /// <list type="bullet">
    /// <item>All data must get initialized in the Awake() or Start() function</item>
    /// <item><c>OnDestroy()</c> will get called a second time once the object really gets destroyed by Unity</item>
    /// <item>If a poolable objects gets parented to none-poolable object, the parent must
    /// be destroyed using <c>ObjectPoolController.Destroy( ... )</c> even if it is none-poolable itself.</item>
    /// <item>If you store a reference to a poolable object then this reference does not evaluate to <c>null</c> after <c>ObjectPoolController.Destroy( ... )</c>
    /// was called like other references to Unity objects normally would. This is because the object still exists - it is just in the pool. 
    /// To make sure that a stored reference to a poolable object is still valid you must use <see cref="PoolableReference{T}"/>.</item>
    /// </list>
    /// </example>
    /// <seealso cref="PoolableObject"/>
    static public class ObjectPoolController
    {

        static public bool isDuringPreload
        {
            get;
            private set;
        }

        // **************************************************************************************************/
        //          public functions
        // **************************************************************************************************/

        /// <summary>
        /// Retrieves an instance of the specified prefab. Either returns a new instance or it claims an instance 
        /// from the pool.
        /// </summary>
        /// <param name="prefab">The prefab to be instantiated.</param>
        /// <returns>
        /// An instance of the prefab.
        /// </returns>
        /// <remarks>
        /// Can be used on none-poolable objects as well. It is good practice to use <c>ObjectPoolController.Instantiate</c>
        /// whenever you may possibly make your prefab poolable in the future.
        /// </remarks>
        /// <seealso cref="Destroy(GameObject)"/>
        static public GameObject Instantiate( GameObject prefab, Transform parent = null )
        {
            PoolableObject prefabPool = prefab.GetComponent<PoolableObject>();
            if ( prefabPool == null )
            {
                //Debug.LogWarning( "Object " + prefab.name + " not poolable " );
            return ( GameObject ) _InstantiateGameObject( prefab, Vector3.zero, Quaternion.identity, parent ); // prefab not pooled, instantiate normally
            }

            GameObject go = _GetPool( prefabPool ).GetPooledInstance( null, null, prefab.activeSelf, parent );
            return go ?? InstantiateWithoutPool( prefab, parent );
        }

        /// <summary>
        /// Retrieves an instance of the specified prefab. Either returns a new instance or it claims an instance
        /// from the pool.
        /// </summary>
        /// <param name="prefab">The prefab to be instantiated.</param>
        /// <param name="position">The position in world coordinates.</param>
        /// <param name="quaternion">The rotation quaternion.</param>
        /// <returns>
        /// An instance of the prefab.
        /// </returns>
        /// <remarks>
        /// Can be used on none-poolable objects as well. It is good practice to use <c>ObjectPoolController.Instantiate</c>
        /// whenever you may possibly make your prefab poolable in the future.
        /// </remarks>
        /// <seealso cref="Destroy(GameObject)"/>
        static public GameObject Instantiate( GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null )
        {
            PoolableObject prefabPool = prefab.GetComponent<PoolableObject>();
            if ( prefabPool == null )
            {
                // no warning displayed by design because this allows to decide later if the object will be poolable or not
                // Debug.LogWarning( "Object " + prefab.name + " not poolable "); 
            return ( GameObject ) _InstantiateGameObject( prefab, position, quaternion, parent ); // prefab not pooled, instantiate normally
            }

            GameObject go = _GetPool( prefabPool ).GetPooledInstance( position, quaternion, prefab.activeSelf, parent );
            return go ?? InstantiateWithoutPool( prefab, position, quaternion, parent );
        }

        /// <summary>
        /// Instantiates the specified prefab without using pooling.
        /// from the pool.
        /// </summary>
        /// <param name="prefab">The prefab to be instantiated.</param>
        /// <returns>
        /// An instance of the prefab.
        /// </returns>
        /// <remarks>
        /// If the prefab is poolable, the <see cref="PoolableObject"/> component will be removed.
        /// This way no warning is generated that a poolable object was created without pooling.
        /// </remarks>
        static public GameObject InstantiateWithoutPool( GameObject prefab, Transform parent = null )
        {
            return InstantiateWithoutPool( prefab, Vector3.zero, Quaternion.identity, parent );
        }

        /// <summary>
        /// Instantiates the specified prefab without using pooling.
        /// from the pool.
        /// </summary>
        /// <param name="prefab">The prefab to be instantiated.</param>
        /// <param name="position">The position in world coordinates.</param>
        /// <param name="quaternion">The rotation quaternion.</param>
        /// <returns>
        /// An instance of the prefab.
        /// </returns>
        /// <remarks>
        /// If the prefab is poolable, the <see cref="PoolableObject"/> component will be removed.
        /// This way no warning is generated that a poolable object was created without pooling.
        /// </remarks>
        static public GameObject InstantiateWithoutPool( GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null )
        {
            _isDuringInstantiate = true;
            GameObject go = _InstantiateGameObject( prefab, position, quaternion, parent ); // prefab not pooled, instantiate normally
            _isDuringInstantiate = false;

            PoolableObject pool = go.GetComponent<PoolableObject>();
            if ( pool != null )
            {
                pool._instantiatedByObjectPoolController = true; // allows disabled game objects with deferred Awake to check if they were instantiated by ObjectPoolController 
                if ( pool.doNotDestroyOnLoad )
                    GameObject.DontDestroyOnLoad( go );

                Component.Destroy( pool );
            }

            return go;
        }

        static GameObject _InstantiateGameObject( GameObject prefab, Vector3 position, Quaternion rotation, Transform parent )
        {
#if UNITY_5_6_OR_NEWER
            var go = (GameObject) GameObject.Instantiate( prefab, position, rotation, parent );
#else
        var go = (GameObject) GameObject.Instantiate( prefab, position, rotation );
        go.transform.parent = parent;
#endif
            return go;
        }

        /// <summary>
        /// Destroys the specified game object, respectively sets the object inactive and adds it to the pool.
        /// </summary>
        /// <param name="obj">The game object.</param>
        /// <remarks>
        /// Can be used on none-poolable objects as well. It is good practice to use <c>ObjectPoolController.Destroy</c>
        /// whenever you may possibly make your prefab poolable in the future. <para/>
        /// Must also be used on none-poolable objects with poolable child objects so the poolable child objects are correctly
        /// moved to the pool.
        /// </remarks>
        /// <seealso cref="Instantiate(GameObject)"/>
        static public void Destroy( GameObject obj ) // destroys poolable and none-poolable objects. Destroys poolable children correctly
        {
            _DetachChildrenAndDestroy( obj.transform, false );
        }

        /// <summary>
        /// Destroys the specified game object, respectively sets the object inactive and adds it to the pool.
        /// </summary>
        /// <param name="obj">The game object.</param>
        /// <remarks>
        /// Can be used on none-poolable objects as well. It is good practice to use <c>ObjectPoolController.Destroy</c>
        /// whenever you may possibly make your prefab poolable in the future. <para/>
        /// Must also be used on none-poolable objects with poolable child objects so the poolable child objects are correctly
        /// moved to the pool.
        /// </remarks>
        /// <seealso cref="Instantiate(GameObject)"/>
        static public void DestroyImmediate( GameObject obj ) // destroys poolable and none-poolable objects. Destroys poolable children correctly
        {
            _DetachChildrenAndDestroy( obj.transform, true );
        }

        /// <summary>
        /// Preloads as many instances to the pool so that there are at least as many as
        /// specified in <see cref="PoolableObject.preloadCount"/>. 
        /// </summary>
        /// <param name="prefab">The prefab.</param>
        /// <remarks>
        /// Use ObjectPoolController.isDuringPreload to check if an object is preloaded in the <c>Awake()</c> function.
        /// If the pool already contains at least <see cref="PoolableObject.preloadCount"/> objects, the function does nothing. 
        /// </remarks>
        /// <seealso cref="PoolableObject.preloadCount"/>
        static public void Preload( GameObject prefab ) // adds as many instances to the prefab pool as specified in the PoolableObject
        {
            PoolableObject poolObj = prefab.GetComponent<PoolableObject>();
            if ( poolObj == null )
            {
                Debug.LogWarning( "Can not preload because prefab '" + prefab.name + "' is not poolable" );
                return;
            }

            var pool = _GetPool( poolObj );

            //check how much Objects need to be preloaded
            int delta = poolObj.preloadCount - pool.GetObjectCount();
            if ( delta <= 0 )
                return;

            isDuringPreload = true;

            bool preloadActive = prefab.activeSelf;

            try
            {
                for ( int i = 0; i < delta; i++ )
                {
                    //dont use prefab.activeSelf because this may change inside Preloadinstance. use the cached value "preloadActive"
                    pool.PreloadInstance( preloadActive );
                }
            }
            finally
            {
                isDuringPreload = false;
            }

            //Debug.Log( "preloaded: " + prefab.name + " " + poolObj.preloadCount + " times" );
        }

        // **************************************************************************************************/
        //          protected / private  functions
        // **************************************************************************************************/

        internal static int _globalSerialNumber = 0;
        internal static bool _isDuringInstantiate = false;

        internal class ObjectPool
        {
            private List<PoolableObject> _pool;
            private GameObject _prefab;
            private PoolableObject _poolableObjectComponent;

            private Transform _poolParent;
            internal Transform poolParent
            {
                get
                {
                    _ValidatePoolParentDummy();
                    return _poolParent;
                }
            }

            public ObjectPool( GameObject prefab )
            {
                this._prefab = prefab;
                this._poolableObjectComponent = prefab.GetComponent<PoolableObject>();
            }

            private void _ValidatePooledObjectDataContainer()
            {
                if ( _pool == null )
                {
                    _pool = new List<PoolableObject>();
                    _ValidatePoolParentDummy();
                }
            }

            private void _ValidatePoolParentDummy()
            {
                if ( !_poolParent )
                {
                    var poolParentDummyGameObject = new GameObject( "POOL:" + _poolableObjectComponent.name );
                    _poolParent = poolParentDummyGameObject.transform;
                    poolParentDummyGameObject._SetActive( false );

                    if ( _poolableObjectComponent.doNotDestroyOnLoad )
                        GameObject.DontDestroyOnLoad( poolParentDummyGameObject );
                }
            }

            internal void Remove( PoolableObject poolObj )
            {
                _pool.Remove( poolObj );
            }

            internal int GetObjectCount()
            {
                return _pool == null ? 0 : _pool.Count;
            }

            internal GameObject GetPooledInstance( Vector3? position, Quaternion? rotation, bool activateObject, Transform parent = null )
            {
                _ValidatePooledObjectDataContainer();

                PoolableObject instance = null;

                for ( int i = 0; i < _pool.Count; i++ )
                {
                    var pooledElement = _pool.ElementAt( i );

                    if ( pooledElement != null && pooledElement._isInPool )
                    {
                        instance = pooledElement;

                        var transform = pooledElement.transform;
                        transform.position = ( position != null ) ? (Vector3) position : _poolableObjectComponent.transform.position;
                        transform.rotation = ( rotation != null ) ? (Quaternion) rotation : _poolableObjectComponent.transform.rotation;
                        transform.localScale = _poolableObjectComponent.transform.localScale;
                        break;
                    }
                }

                if ( instance == null && _pool.Count < _poolableObjectComponent.maxPoolSize ) //create and return new element
                {
                    instance = _NewPooledInstance( position, rotation, activateObject, false );
                instance.transform.parent = parent;
                    return instance.gameObject;
                }

                if ( instance != null )
                {
                    instance.TakeFromPool( parent, activateObject );
                    return instance.gameObject;
                }
                else
                    return null;
            }

            internal PoolableObject PreloadInstance( bool preloadActive )
            {
                _ValidatePooledObjectDataContainer();

                PoolableObject poolObj = _NewPooledInstance( null, null, preloadActive, true );

                return poolObj;
            }

            private PoolableObject _NewPooledInstance( Vector3? position, Quaternion? rotation, bool createActive, bool addToPool )
            {
                _isDuringInstantiate = true;

                _prefab._SetActive( false );

                GameObject go = (GameObject) GameObject.Instantiate(
                    _prefab,
                    position ?? Vector3.zero,
                    rotation ?? Quaternion.identity
                    );

                _prefab._SetActive( true );

                PoolableObject poolObj = go.GetComponent<PoolableObject>();

                poolObj._pool = this;
                poolObj._serialNumber = ++_globalSerialNumber;
                poolObj.name += poolObj._serialNumber;

                if ( poolObj.doNotDestroyOnLoad )
                    GameObject.DontDestroyOnLoad( poolParent );

                _pool.Add( poolObj );

                if ( addToPool )
                {
                    poolObj._PutIntoPool();
                }
                else
                {
                    poolObj._usageCount++;

                    if ( createActive )
                    {
                        go.SetActive( true );

                        if ( poolObj.sendPoolableActivateDeactivateMessages )
                        {
                            CallMethodOnObject( poolObj.gameObject, "OnPoolableObjectActivated", true, true, poolObj.useReflectionInsteadOfMessages );
                        }
                    }
                }

                _isDuringInstantiate = false;

                return poolObj;
            }

            /// <summary>
            /// Deactivate all active pooled objects
            /// </summary>
            internal int _SetAllAvailable()
            {
                int count = 0;
                for ( int i = 0; i < _pool.Count; i++ )
                {
                    var element = _pool.ElementAt( i );

                    if ( element != null && !element._isInPool )
                    {
                        element._PutIntoPool();
                        count++;
                    }
                }
                return count;
            }

            internal void CallMethodOnObject( GameObject obj, string method, bool includeChildren, bool includeInactive, bool useReflection )
            {
                if ( useReflection )
                {
                    if ( includeChildren )
                        obj.InvokeMethodInChildren( method, includeInactive );
                    else
                        obj.InvokeMethod( method, includeInactive );
                }
                else
                {
                    if ( !obj._GetActive() )
                        Debug.LogWarning( "Tried to call method \"" + method + "\" on an inactive GameObject using Unity-Messaging-System. This only works on active GameObjects and Components! Check \"useReflectionInsteadOfMessages\"!", obj );

                    if ( includeChildren )
                        obj.BroadcastMessage( method, null, SendMessageOptions.DontRequireReceiver );
                    else
                        obj.SendMessage( method, null, SendMessageOptions.DontRequireReceiver );
                }
            }
        }

        static private Dictionary<int, ObjectPool> _pools = new Dictionary<int, ObjectPool>();

        static internal ObjectPool _GetPool( PoolableObject prefabPoolComponent )
        {
            ObjectPool pool;

            GameObject prefab = prefabPoolComponent.gameObject;

            var instanceID = prefab.GetInstanceID();
            if ( !_pools.TryGetValue( instanceID, out pool ) )
            {
                pool = new ObjectPool( prefab );
                _pools.Add( instanceID, pool );
            }

            return pool;
        }

        static private void _DetachChildrenAndDestroy( Transform transform, bool destroyImmediate )
        {
            var po = transform.GetComponent<PoolableObject>();

            if ( transform.childCount > 0 )
            {
                List<PoolableObject> poolableChilds = new List<PoolableObject>();
                transform.GetComponentsInChildren<PoolableObject>( true, poolableChilds );

                if ( po != null )
                    poolableChilds.Remove( po );

                //first destroy all poolable childs.
                for ( int i = 0; i < poolableChilds.Count; i++ )
                {
                    if ( poolableChilds[ i ] == null || poolableChilds[ i ]._isInPool ) continue; //can happen when a poolable is a child of another poolable

                    if ( destroyImmediate )
                        ObjectPoolController.DestroyImmediate( poolableChilds[ i ].gameObject );
                    else
                        ObjectPoolController.Destroy( poolableChilds[ i ].gameObject );
                }
            }

            if ( po != null )
            {
                //move poolable Object to pool
                po._PutIntoPool();
            }
            else
            {
                //destroy non-poolable object itself
                if ( destroyImmediate )
                    GameObject.DestroyImmediate( transform.gameObject );
                else
                    GameObject.Destroy( transform.gameObject );
            }
        }
    }

    /// <summary>
    /// Auxiliary class to overcome the problem of references to pooled objects that should become <c>null</c> when 
    /// objects are moved back to the pool after calling <see cref="ObjectPoolController.Destroy(GameObject)"/>.
    /// </summary>
    /// <typeparam name="T">A <c>UnityEngine.Component</c></typeparam>
    /// <example>
    /// Instead of a normal reference to a script component on a poolable object use 
    /// <code>
    /// MyScriptComponent scriptComponent = PoolableObjectController.Instantiate( prefab ).GetComponent&lt;MyScriptComponent&gt;();
    /// var myReference = new PoolableReference&lt;MyScriptComponent&gt;( scriptComponent );
    /// if( myReference.Get() != null ) // will check if poolable instance still belongs to the original object
    /// {
    ///     myReference.Get().MyComponentFunction();
    /// }
    /// </code>
    /// </example>
    public class PoolableReference<T> where T : Component
    {
        PoolableObject _pooledObj;
        int _initialUsageCount;

#if REDUCED_REFLECTION
    Component _objComponent;
#else
        T _objComponent;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolableReference&lt;T&gt;"/> class with a <c>null</c> reference.
        /// </summary>
        public PoolableReference()
        {
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolableReference&lt;T&gt;"/> class with the specified reference.
        /// </summary>
        /// <param name="componentOfPoolableObject">The referenced component of the poolable object.</param>
#if REDUCED_REFLECTION
    public PoolableReference( Component componentOfPoolableObject )
#else
        public PoolableReference( T componentOfPoolableObject )
#endif
        {
            Set( componentOfPoolableObject, false );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolableReference&lt;T&gt;"/> class from 
        /// a given <see cref="PoolableReference&lt;T&gt;"/>.
        /// </summary>
        /// <param name="poolableReference">The poolable reference.</param>
        public PoolableReference( PoolableReference<T> poolableReference )
        {
            _objComponent = poolableReference._objComponent;
            _pooledObj = poolableReference._pooledObj;
            _initialUsageCount = poolableReference._initialUsageCount;
        }

        /// <summary>
        /// Resets the reference to <c>null</c>.
        /// </summary>
        public void Reset()
        {
            _pooledObj = null;
            _objComponent = null;
            _initialUsageCount = 0;
        }

        /// <summary>
        /// Gets the reference to the script component, or <c>null</c> if the object was 
        /// already destroyed or moved to the pool.
        /// </summary>
        /// <returns>
        /// The reference to <c>T</c> or null
        /// </returns>
        public T Get()
        {
            if ( !_objComponent ) return null;

            if ( _pooledObj ) // could be set to a none-poolable object
            {
                if ( _pooledObj._usageCount != _initialUsageCount || _pooledObj._isInPool )
                {
                    _objComponent = null;
                    _pooledObj = null;
                    return null;
                }
            }
            return (T) _objComponent;
        }

#if REDUCED_REFLECTION
    public void Set( Component componentOfPoolableObject, bool allowNonePoolable )
#else
        public void Set( T componentOfPoolableObject )
        {
            Set( componentOfPoolableObject, false );
        }

        /// <summary>
        /// Sets the reference to a poolable object with the specified component.
        /// </summary>
        /// <param name="componentOfPoolableObject">The component of the poolable object.</param>
        /// <param name="allowNonePoolable">If set to false an error is output if the object does not have the <see cref="PoolableObject"/> component.</param>
        public void Set( T componentOfPoolableObject, bool allowNonePoolable )
#endif
        {
            if ( !componentOfPoolableObject )
            {
                Reset();
                return;
            }
            _objComponent = (T) componentOfPoolableObject;
            _pooledObj = _objComponent.GetComponent<PoolableObject>();
            if ( !_pooledObj )
            {
                if ( allowNonePoolable )
                {
                    _initialUsageCount = 0;
                }
                else
                {
                    Debug.LogError( "Object for PoolableReference must be poolable" );
                    return;
                }
            }
            else
            {
                _initialUsageCount = _pooledObj._usageCount;
            }
        }
    }
}