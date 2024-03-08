using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [Header("Modifiers")]
    [SerializeField] float _movementSpeed;
    [SerializeField] float _movementSpeedFollow;
    [SerializeField] float _raycastDistance;
    [SerializeField] float _groundRaycast;
    [Header("Animation names")]
    [SerializeField] string _AttackAnimationName;
    [SerializeField] string _IdleAnimationName;
    [SerializeField] string _RunAnimationName;

    [Header("Cooldowns")]
    [SerializeField] float _targetingPlayerCooldown;
    [SerializeField] float _standingStillCoolDown;
    [SerializeField] float _roamingCoolDown;

    [Header("LayerMasks")]
    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] LayerMask _groundLayerMask;

    //cashed
    Animator _animator;
    Rigidbody2D _rb;
    BoxCollider2D _boxCollider;
    Player _player;
    SpriteRenderer _sr;

    //Vectors
    [Header("Start heading")]
    [SerializeField] Vector3 _lookRightOrleft;

    //bools unused :'( feel free to delete
    bool _onFollowingPlayer;
    bool _onRandomizedBehavior;
    bool _onRoaming;
    bool _attacking;

    private enum STATES { idle, roaming, attacking, followingPlayer };
    private STATES currentState;

    private void Start()
    {
        Cashed();
    }
    protected void Cashed()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _sr = GetComponent<SpriteRenderer>();
        _player = FindObjectOfType<Player>();
        currentState = STATES.roaming;
    }



    private void Update()
    {
        RaycastSystem();
        AnimationSystem();
        switch (currentState)
        {
            case STATES.idle:
                IdleBheavior();
                break;
            case STATES.roaming:
                roamingBehavior();
                break;
            case STATES.attacking:
                AttackingBheavior();
                break;
            case STATES.followingPlayer:
                FollowingPlayerBehavior();
                    break;
        }
        
    }

    private void RaycastSystem()
    {
        // IT WAS THE RAYCAST HAHAHA OMG, keep this for the attack behavior, somehow raycast makes the enemy not attacking
        if (currentState == STATES.followingPlayer || currentState == STATES.attacking)
        {
            return;
        }
        /////////////////////////////////////////GROUND////////////////////////////////////////////////////////////////
        RaycastHit2D hitG = Physics2D.Raycast(_boxCollider.bounds.center, _lookRightOrleft, _groundRaycast, _groundLayerMask);
        Debug.DrawRay(_boxCollider.bounds.center, _lookRightOrleft * _groundRaycast, Color.red);

        if (hitG)
        {
            Debug.DrawRay(_boxCollider.bounds.center, _lookRightOrleft * _groundRaycast, Color.green);
            _lookRightOrleft = -_lookRightOrleft;
        }
        ///////////////////////////////////////////////////////////////PLAYER/////////////////////////////////////////
        RaycastHit2D hitP = Physics2D.Raycast(_boxCollider.bounds.center, _lookRightOrleft, _raycastDistance, _playerLayerMask);
        Debug.DrawRay(_boxCollider.bounds.center, _lookRightOrleft * _raycastDistance, Color.yellow);
        if(hitP)
        {
            Debug.DrawRay(_boxCollider.bounds.center, _lookRightOrleft * _raycastDistance, Color.blue);
            currentState = STATES.followingPlayer;
        }

    }

    private void FollowingPlayerBehavior()
    {
        Debug.Log(Vector2.Distance(transform.position, _player.transform.position));
        if(transform.position.x > _player.transform.position.x) // if player is on left we move left
        {
            _rb.AddForce(Vector2.left * _movementSpeedFollow, ForceMode2D.Impulse);
            _sr.flipX = true;
        }
        if( transform.position.x < _player.transform.position.x) // if player is on right we move right
        {
            _rb.AddForce(Vector2.right * _movementSpeedFollow, ForceMode2D.Impulse);
            _sr.flipX = false;
        }
        if(Vector2.Distance(transform.position, _player.transform.position) < 0.5f)
        {
            _rb.velocity = Vector2.zero;
            currentState = STATES.attacking;
        }
        if (Vector2.Distance(transform.position, _player.transform.position) > 5f)
        {
            currentState = STATES.roaming;
        }

        if(_rb.velocity.magnitude > _movementSpeedFollow)
        {
            _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, _movementSpeedFollow);
        }
    }

    private void AttackingBheavior()
    {
        if (Vector2.Distance(transform.position, _player.transform.position) > 1f)
        {
            currentState = STATES.followingPlayer;
        }
    }

    private void roamingBehavior()
    {
        _rb.AddForce(_lookRightOrleft, ForceMode2D.Impulse);
        if (_rb.velocity.magnitude > _movementSpeed) 
        {
            _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, _movementSpeed);
        }
        if(_lookRightOrleft.x > 0)
        {
            _sr.flipX = false;
        }
        else if (_lookRightOrleft.x <  0) { 
                _sr.flipX = true;

        }
    }

    private void IdleBheavior()
    {
        _rb.velocity = Vector2.zero;
    }


    private void AnimationSystem()
    {
        if(currentState == STATES.roaming || currentState == STATES.followingPlayer)
        {
            _animator.Play(_RunAnimationName);
        }
        if(currentState == STATES.idle)
        {
            _animator.Play(_IdleAnimationName);
        }
        if (currentState == STATES.attacking)
        {
            _animator.Play(_AttackAnimationName);
        }
    }
}



