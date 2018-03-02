using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor2D : MonoBehaviour {

    const float HALF_SIZE = 0.8f;

    [SerializeField] float walkSpeed;
    [SerializeField] float airForce;
    [SerializeField] float jumpImpulse;
    [SerializeField] float airResistance;
    [SerializeField] float ladderSpeed;

    [SerializeField] LayerMask terrainMask;
    [SerializeField] LayerMask ladderMask;

    // GENERAL MOVEMENT COMMANDS
    [HideInInspector] public bool goLeft;
    [HideInInspector] public bool goRight;
    [HideInInspector] public bool jump;

    // ONEWAYS COMMANDS
    [HideInInspector] public bool drop;

    // LATTER COMMANDS
    [HideInInspector] public bool holdLadder;
    [HideInInspector] public bool goLeftLadder;
    [HideInInspector] public bool goRightLadder;
    [HideInInspector] public bool goUpLadder;
    [HideInInspector] public bool goDownLadder;

    // SITUATION
    public bool isGrounded { get; private set; }
    public bool isOnOneway { get; private set; }
    public bool isOnLadder { get; private set; }

    // ONEWAY
    EdgeCollider2D[] oneways;
    int onewayInd;
    int onewayFrame;

    // JUMP
    float lastJump;

    // LADDER
    Collider2D ladder;

    Rigidbody2D body;
    BoxCollider2D collider;

	void Start () {
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        oneways = new EdgeCollider2D[3];
    }

    void Update() {
        CheckBottom();

        if (holdLadder) {
            CheckLadder();
        } else {
            if (ladder != null) {
                transform.SetParent(null);
                ladder = null;

                isOnLadder = false;
            }
        }

        if (isOnLadder) {
            float xSpeed;
            float ySpeed;

            if (goLeftLadder && !goRightLadder) {
                xSpeed = -ladderSpeed;
            } else if(!goLeftLadder && goRightLadder) {
                xSpeed = ladderSpeed;
            } else {
                xSpeed = 0;
            }

            if (goDownLadder && !goUpLadder) {
                ySpeed = -ladderSpeed;
            } else if (!goDownLadder && goUpLadder) {
                ySpeed = ladderSpeed;
            } else {
                ySpeed = 0;
            }
            
            body.velocity = new Vector2(xSpeed, ySpeed);

            body.gravityScale = 0;
        } else {
            body.gravityScale = 2;

            if (isGrounded) {
                if (goLeft && !goRight) {
                    body.velocity = new Vector2(-walkSpeed, body.velocity.y);
                } else if (goRight && !goLeft) {
                    body.velocity = new Vector2(walkSpeed, body.velocity.y);
                } else {
                    body.velocity = new Vector2(0, body.velocity.y);
                }

                if (jump && Time.time - lastJump > 0.2f && body.velocity.y < 0.1f) {
                    lastJump = Time.time;
                    body.velocity = body.velocity + jumpImpulse * Vector2.up;
                }

                if (isOnOneway && drop && body.velocity.y > -0.1f) {
                    transform.position += 0.01f * Vector3.down;

                    for (int i = 0; i < onewayInd; i++) {
                        Physics2D.IgnoreCollision(collider, oneways[i], true);
                    }

                    onewayFrame = 60;

                    drop = false;
                }
            } else {
                if (goLeft && !goRight) {
                    body.AddForce(airForce * Vector2.left);
                }
                if (goRight && !goLeft) {
                    body.AddForce(airForce * Vector2.right);
                }

                body.AddForce(-airResistance * body.velocity.x * Vector2.right);
            }

            if (onewayFrame > 0) {
                onewayFrame--;

                if (onewayFrame == 0) {
                    for (int i = 0; i < onewayInd; i++) {
                        Physics2D.IgnoreCollision(collider, oneways[i], false);
                    }

                    onewayInd = 0;
                }
            }
        }
    }

    void CheckBottom() {
        isGrounded = false;
        isOnOneway = true;

        int count = 4;
        float rayLength = 0.05f;

        if(onewayFrame == 0) onewayInd = 0;

        for (int i = -count; i <= count; i++) {
            float ratio = 0.99f * HALF_SIZE * (i / (float)count);
            Vector2 origin = transform.TransformPoint(ratio * Vector2.left + (HALF_SIZE + 0.01f) * Vector2.down);
            Vector2 direction = Vector2.down;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, terrainMask);

            if(hit) {
                isGrounded = true;

                if (!(hit.collider is EdgeCollider2D)) {
                    isOnOneway = false;
                } else {
                    if(onewayFrame == 0 && onewayInd < 3) {
                        bool notConsidered = true;
                        for(int j = 0; j < onewayInd; j++) {
                            if(oneways[j] == hit.collider) {
                                notConsidered = false;
                                break;
                            }
                        }

                        if (notConsidered) {
                            oneways[onewayInd] = hit.collider as EdgeCollider2D;
                            onewayInd++;
                        }
                    }
                }
            }
        }

        isOnOneway = isGrounded && isOnOneway;
    }

    void CheckLadder() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector3(0, 0, 1), 20, ladderMask);

        if (hit) {
            if(hit.collider != ladder) {
                ladder = hit.collider;
                transform.SetParent(ladder.transform);
            }

            isOnLadder = true;
        } else {
            if(ladder != null) {
                ladder = null;
                transform.SetParent(null);
            }

            isOnLadder = false;
        }
    }
}
