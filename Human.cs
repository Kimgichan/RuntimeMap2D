using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    public float speed;
    public Sprite[] moveAnimation;
    public Sprite[] standAnimation;
    public Sprite[] stairAnimation;
    public Sprite[] airAnimation;
    public Sprite[] railTouchedAnimation;
    public float frame;

    [SerializeField]
    private Map map;
    public float weight;
    public float airFric;
    private float airSpeedX;
    protected SpriteRenderer objectSprite;
    protected float delay;
    protected int animationIndx = 0;

    //protected DelegateMoveEvent dMoveEvent;
    //protected Dictionary<byte, DelegateMoveEvent> eventDictionary;
    protected Vector2 nextPos;
    protected float diagonalSpeed;

    protected Action RIGHT_WALK;
    protected Action LEFT_WALK;
    private static readonly EmptyFailStartForm emptyFailStartForm = new EmptyFailStartForm();
    private static readonly EmptyFailRunForm emptyFailRunForm = new EmptyFailRunForm();
    private static readonly EmptyFailEndForm emptyFailEndForm = new EmptyFailEndForm();
    private static readonly JumpReadyForm jumpReadyForm = new JumpReadyForm();
    private static readonly JumpRunForm jumpRunForm = new JumpRunForm();
    private static readonly JumpEndForm jumpEndForm = new JumpEndForm();
    private static readonly GroundFlatLandForm groundFlatLandForm = new GroundFlatLandForm();
    private static readonly GroundFlatForm groundFlatForm = new GroundFlatForm();
    private static readonly GroundUpHillLandForm groundUpHillLandForm = new GroundUpHillLandForm();
    private static readonly GroundUpHillForm groundUpHillForm = new GroundUpHillForm();
    private static readonly GroundDownHillLandForm groundDownHillLandForm = new GroundDownHillLandForm();
    private static readonly GroundDownHillForm groundDownHillForm = new GroundDownHillForm();
    private static readonly RailTouchStandForm railTouchStandForm = new RailTouchStandForm();
    private static readonly RailTouchJumpReadyForm railTouchJumpReadyForm = new RailTouchJumpReadyForm();
    private static readonly GroundLandContact groundLandContact = new GroundLandContact();
    private static readonly GroundAirContact groundAirContact = new GroundAirContact();
    private static readonly GroundRailTouchContact groundRailTouchContact = new GroundRailTouchContact();

    // Start is called before the first frame update
    void Start()
    {
        //RIGHT_WALK = new Action(CheckStartD);
        //LEFT_WALK = new Action(CheckStartA);
        //moveHandleCheck = new Action(CheckStand);
        //moveHandleAction = new Action(ActionStand);
        diagonalSpeed = speed / Mathf.Sqrt(2);
        objectSprite = this.gameObject.GetComponent<SpriteRenderer>();
        humanUpdateForm = Human.groundFlatForm;
        humanContact = Human.groundLandContact;
        delay = frame;
        moveInput = new MoveInput();
        moveInput.checkInput = new Action(CheckStand);
        moveInput.actionInput = new Action(ActionStand);
    }

    //애니메이션
    protected void StandAnimation()
    {
        if (animationIndx != 0) animationIndx = 0;
        objectSprite.sprite = standAnimation[0];
    }
    protected void MoveAnimation()
    {
        delay -= currentDelta;
        if (delay < 0f)
        {
            delay = frame;
            objectSprite.sprite = moveAnimation[animationIndx++];
            if (animationIndx == moveAnimation.Length) animationIndx = 0;
        }
    }

    protected void StairUpAnimation()
    {
        delay -= currentDelta;
        if (delay < 0f)
        {
            delay = frame;
            objectSprite.sprite = stairAnimation[animationIndx];
            animationIndx += 2;
            if (animationIndx >= stairAnimation.Length) animationIndx = 0;
        }
    }
    protected void StairDownAnimation()
    {
        delay -= currentDelta;
        if (delay < 0f)
        {
            delay = frame;
            objectSprite.sprite = stairAnimation[animationIndx++];
            if (animationIndx == stairAnimation.Length) animationIndx = 1;
        }
    }
    protected float currentDelta;
    protected void TurnRight()
    {
        if (objectSprite.flipX)
        {
            objectSprite.flipX = false;
            this.transform.position = new Vector3(this.transform.position.x + 1f, this.transform.position.y, this.transform.position.z);
        }
    }
    protected void TurnLeft()
    {
        if (!objectSprite.flipX)
        {
            objectSprite.flipX = true;
            this.transform.position = new Vector3(this.transform.position.x - 1f, this.transform.position.y, this.transform.position.z);
        }
    }
    protected void CheckStand()
    {
        nextPos = Vector2.zero;
        moveInput.checkInput = NullFunc;
        moveInput.actionInput = ActionStand;
    }

    // Update is called once per frame
    void Update()
    {
        this.currentDelta = Time.deltaTime;
        this.humanUpdateForm.HumanUpdate(this);
    }
    protected enum EBackGroundForm
    {
        GROUND = 0,
        WATER
    }
    protected enum EInputForm
    {
        FAIL = 0,
        FLAT,
        UPHILL,
        DOWNHILL,
        RailTouch
    }

    public float airSpeedC;
    protected interface IHumanUpdateForm
    {
        void UpdateInit(Human human);
        void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human);
        void HumanUpdate(Human human);
        void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm);
    }
    protected IHumanUpdateForm humanUpdateForm;
    protected void NullFunc()
    {

    }
    private class EmptyFailStartForm : EmptyFailRunForm
    { 
        public override void UpdateInit(Human human)
        {
            human.airSpeed = 0f;
            if (human.moveInput.input.Count > 0)
            {
                if (human.moveInput.input[0] == MoveInput.Left)
                {
                    human.moveInput.checkInput = human.CheckAir_MoveLeft;
                    human.moveInput.actionInput = human.ActionAir_Move;
                }
                else
                {
                    human.moveInput.checkInput = human.CheckAir_MoveRight;
                    human.moveInput.actionInput = human.ActionAir_Move;
                }
            }
            else
            {
                human.moveInput.checkInput = human.CheckAir_Stop;
                human.moveInput.actionInput = human.ActionAir_Move;
            }
            human.humanContact = Human.groundAirContact;
            human.objectSprite.sprite = human.airAnimation[3];
        }

        protected override bool AirNextUpdate(Human human)
        {
            float speedDic = human.airSpeed + human.airSpeedC;
            if (speedDic > human.weight / 3f)
            {
                human.humanUpdateForm = Human.emptyFailRunForm;
                Human.emptyFailRunForm.UpdateInit(human);
                //Human.emptyFailRunForm.HumanUpdate(human);
                return true;
            }
            human.airSpeed = speedDic;
            human.nextPos.y = -(human.currentDelta * human.airSpeed);
            return false;
        }
    }

    private class EmptyFailEndForm : EmptyFailRunForm
    {
        public override void HumanUpdate(Human human)
        {
            Human.UpdateMoveInputMacro(human, KeyCode.A, MoveInput.Left, ref human.moveInput.pressLeft, human.CheckAir_MoveLeft, human.CheckAir_Stop, human.CheckAir_MoveRight);
            Human.UpdateMoveInputMacro(human, KeyCode.D, MoveInput.Right, ref human.moveInput.pressRight, human.CheckAir_MoveRight, human.CheckAir_Stop, human.CheckAir_MoveLeft);

            human.nextPos.y = -(human.currentDelta * human.weight);
            if (human.airSpeedX > 0f)
            {
                human.airSpeedX -= human.airFric;
                if (human.airSpeedX < 0f) human.airSpeedX = 0f;
            }
            human.moveInput.checkInput();
            //map 충돌체크
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            
            human.moveInput.actionInput();
        }
    }

    private class EmptyFailRunForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.objectSprite.sprite = human.airAnimation[4];
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if(eBGForm == EBackGroundForm.GROUND)
            {
                if(eIForm == EInputForm.FLAT)
                {
                    human.humanUpdateForm = Human.groundFlatLandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.UPHILL)
                {
                    human.humanUpdateForm = Human.groundUpHillLandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.DOWNHILL)
                {
                    human.humanUpdateForm = Human.groundDownHillLandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
            else
            {
                if(eIForm == EInputForm.FAIL)
                {
                }
                else if (eIForm == EInputForm.FLAT)
                {
                }
                else if (eIForm == EInputForm.UPHILL)
                {
                }
                else if (eIForm == EInputForm.DOWNHILL)
                {
                }
            }
        }
        
        public virtual void HumanUpdate(Human human)
        {
            //각 EmptyFail 관련 무브 함수로 수정예정
            Human.UpdateMoveInputMacro(human, KeyCode.A, MoveInput.Left, ref human.moveInput.pressLeft, human.CheckAir_MoveLeft, human.CheckAir_Stop, human.CheckAir_MoveRight);
            Human.UpdateMoveInputMacro(human, KeyCode.D, MoveInput.Right, ref human.moveInput.pressRight, human.CheckAir_MoveRight, human.CheckAir_Stop, human.CheckAir_MoveLeft);

            if (AirNextUpdate(human)) return;
            if (human.airSpeedX > 0f)
            {
                human.airSpeedX -= human.airFric;
                if (human.airSpeedX < 0f) human.airSpeedX = 0f;
            }
            human.moveInput.checkInput();
            //map 충돌체크
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.moveInput.actionInput(); 
        }

        protected virtual bool AirNextUpdate(Human human)
        {
            if ((human.airSpeed += human.airSpeedC) > human.weight)
            {
                human.airSpeed = human.weight;
                human.humanUpdateForm = Human.emptyFailEndForm;
                return true;
            }
            human.nextPos.y = -(human.currentDelta * human.airSpeed);
            return false;
        }

        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.FAIL;
        }
    }
    protected float airSpeed;
    protected void CheckAir_MoveLeft()
    {
        TurnLeft();
        nextPos.x = -(currentDelta * airSpeedX);
        CheckAir_Move();
    }
    protected void CheckAir_MoveRight()
    {
        TurnRight();
        nextPos.x = currentDelta * airSpeedX;
        CheckAir_Move();
    }
    protected void CheckAir_Stop()
    {
        nextPos.x = 0f;
        CheckAir_Move();
    }
    protected void CheckAir_Move()
    {
        //float actualSpeed = Math.Abs(nextPos.y);
        //nextPos.Normalize();
        //nextPos *= actualSpeed;
        this.moveInput.actionInput = ActionAir_Move;
    }
    protected void ActionAir_Move()
    {
        this.gameObject.transform.position = new Vector3(this.transform.position.x + nextPos.x, this.transform.position.y + nextPos.y, this.transform.position.z);
        nextPos = Vector2.zero;
    }

    private class JumpRunForm : EmptyFailRunForm
    {
        public override void UpdateInit(Human human)
        {
            human.humanContact = Human.groundAirContact;
            human.objectSprite.sprite = human.airAnimation[1];
            if (human.moveInput.input.Count > 0)
            {
                if (human.moveInput.input[0] == MoveInput.Left)
                {
                    human.moveInput.checkInput = human.CheckAir_MoveLeft;
                    human.moveInput.actionInput = human.ActionAir_Move;
                }
                else
                {
                    human.moveInput.checkInput = human.CheckAir_MoveRight;
                    human.moveInput.actionInput = human.ActionAir_Move;
                }
            }
            else
            {
                human.moveInput.checkInput = human.CheckAir_Stop;
                human.moveInput.actionInput = human.ActionAir_Move;
            }
        }
        public override void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if(eIForm == EInputForm.FAIL)
                {
                    human.humanUpdateForm = Human.emptyFailStartForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if (eIForm == EInputForm.FLAT)
                {
                }
                else if (eIForm == EInputForm.UPHILL)
                {
                }
                else if (eIForm == EInputForm.DOWNHILL)
                {
                }
            }
            else
            {
                if (eIForm == EInputForm.FAIL)
                {
                }
                else if (eIForm == EInputForm.FLAT)
                {
                }
                else if (eIForm == EInputForm.UPHILL)
                {
                }
                else if (eIForm == EInputForm.DOWNHILL)
                {
                }
            }
        }
        protected override bool AirNextUpdate(Human human)
        {
            if ((human.airSpeed -= human.airSpeedC) < human.weight*0.3333f)
            {
                human.humanUpdateForm = Human.jumpEndForm;
                human.humanUpdateForm.UpdateInit(human);
                //human.humanUpdateForm.HumanUpdate(human);
                return true;
            }
            human.nextPos.y = human.currentDelta * human.airSpeed;
            return false;
        }
    }
    private class JumpEndForm : JumpRunForm
    {
        public override void UpdateInit(Human human)
        {
            human.objectSprite.sprite = human.airAnimation[2];
            human.humanContact = Human.groundRailTouchContact;
        }
        public override void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    human.humanUpdateForm = Human.emptyFailStartForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.RailTouch)
                {
                    human.humanUpdateForm = Human.railTouchStandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if (eIForm == EInputForm.FLAT)
                {
                }
                else if (eIForm == EInputForm.UPHILL)
                {
                }
                else if (eIForm == EInputForm.DOWNHILL)
                {
                }
            }
            else
            {
                if (eIForm == EInputForm.FAIL)
                {
                }
                else if (eIForm == EInputForm.FLAT)
                {
                }
                else if (eIForm == EInputForm.UPHILL)
                {
                }
                else if (eIForm == EInputForm.DOWNHILL)
                {
                }
            }
        }
        protected override bool AirNextUpdate(Human human)
        {
            if((human.airSpeed -= human.airSpeedC) < 0)
            {
                human.humanUpdateForm = Human.emptyFailStartForm;
                human.humanUpdateForm.UpdateInit(human);
                return true;
            }
            human.nextPos.y = human.currentDelta * human.airSpeed;
            return false;
        }
    }

    protected IHumanUpdateForm beforeUpdateForm;
    private class JumpReadyForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.airSpeedX = human.speed;
            human.humanContact = Human.groundLandContact;
            human.objectSprite.sprite = human.airAnimation[0];
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    human.airSpeed = 0f;
                    Human.emptyFailStartForm.UpdateInit(human);
                    human.humanUpdateForm = Human.emptyFailStartForm;
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if (human.airSpeed > human.weight)
            {
                human.airSpeed = human.weight;
                human.humanUpdateForm = Human.jumpRunForm;
                human.humanUpdateForm.UpdateInit(human);
                //human.humanUpdateForm.HumanUpdate(human);
                Vector2 halfSize = human.objectSprite.size * 0.5f;
                Vector2 min = new Vector2(human.transform.position.x - halfSize.x, human.transform.position.y - halfSize.y);
                Vector2 max = new Vector2(human.transform.position.x + halfSize.x, human.transform.position.y + halfSize.y);
                human.map.CrushCheck(in min, in max, human);
                return;
            }
            if (!Input.GetKey(KeyCode.J))
            {
                if (human.airSpeed > human.weight * 0.25f)
                {
                    human.humanUpdateForm = Human.jumpRunForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else
                {
                    human.humanUpdateForm = human.beforeUpdateForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                Vector2 halfSize = human.objectSprite.size * 0.5f;
                Vector2 min = new Vector2(human.transform.position.x - halfSize.x, human.transform.position.y - halfSize.y);
                Vector2 max = new Vector2(human.transform.position.x + halfSize.x, human.transform.position.y + halfSize.y);
                human.map.CrushCheck(in min, in max, human);
                return;
            }
            human.airSpeed += human.airSpeedC; 
        }
        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            human.beforeUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
        }
    }

    private class GroundFlatLandForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.airSpeedX = 0f;
            human.humanContact = Human.groundLandContact;
            human.objectSprite.sprite = human.airAnimation[5];
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if(eBGForm == EBackGroundForm.GROUND)
            {
                if(eIForm == EInputForm.FAIL)
                {
                    Human.emptyFailStartForm.UpdateInit(human);
                    human.humanUpdateForm = Human.emptyFailStartForm;
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if(human.airSpeed < 0f)
            {
                human.airSpeed = 0f;
                Human.groundFlatForm.UpdateInit(human);
                human.humanUpdateForm = Human.groundFlatForm;
                return;
            }
            human.airSpeed -= human.airSpeedC*0.8f;
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x, human.transform.position.y - halfSize.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x, human.transform.position.y + halfSize.y);
            human.map.CrushCheck(in min, in max, human);
        }
        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.FLAT;
        }
    }

    private class GroundFlatForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.airSpeed = 0f;
            if (human.moveInput.input.Count > 0)
            {
                if (human.moveInput.input[0] == MoveInput.Left)
                {
                    human.moveInput.checkInput = human.CheckStartA;
                    human.moveInput.actionInput = human.ActionStartA;
                }
                else
                {
                    human.moveInput.checkInput = human.CheckStartD;
                    human.moveInput.actionInput = human.ActionStartD;
                }
            }
            else
            {
                human.moveInput.checkInput = human.CheckStand;
                human.moveInput.actionInput = human.ActionStand;
            }
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    Human.emptyFailStartForm.UpdateInit(human);
                    human.humanUpdateForm = Human.emptyFailStartForm;
                }
                else if(eIForm == EInputForm.UPHILL)
                {
                    human.humanUpdateForm = Human.groundUpHillForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.DOWNHILL)
                {
                    human.humanUpdateForm = Human.groundDownHillForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if (Input.GetKey(KeyCode.J))
            {
                human.beforeUpdateForm = Human.groundFlatForm;
                human.humanUpdateForm = Human.jumpReadyForm;
                human.humanUpdateForm.UpdateInit(human);
                //human.humanUpdateForm.HumanUpdate(human);
                return;
            }
            Human.UpdateMoveInputMacro(human, KeyCode.A, MoveInput.Left, ref human.moveInput.pressLeft, human.CheckStartA, human.CheckStand, human.CheckStartD);
            Human.UpdateMoveInputMacro(human, KeyCode.D, MoveInput.Right, ref human.moveInput.pressRight, human.CheckStartD, human.CheckStand, human.CheckStartA);
            human.moveInput.checkInput();
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.moveInput.actionInput();
        }
        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.FLAT;
        }
    }
    private class GroundUpHillLandForm : IHumanUpdateForm
    {
        public void UpdateInit(Human human)
        {
            human.humanContact = Human.groundLandContact;
            if (human.objectSprite.flipX)
            {
                human.objectSprite.sprite = human.stairAnimation[1];
            }
            else
            {
                human.objectSprite.sprite = human.stairAnimation[0];
            }
        }
        public void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if(eBGForm == EBackGroundForm.GROUND)
            {
                if(eIForm == EInputForm.FAIL)
                {
                    human.humanUpdateForm = Human.emptyFailStartForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.FLAT)
                {
                    human.humanUpdateForm = Human.groundFlatLandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public void HumanUpdate(Human human)
        {
            if (human.airSpeed < 0f)
            {
                human.airSpeed = 0f;
                human.humanUpdateForm = Human.groundUpHillForm;
                human.humanUpdateForm.UpdateInit(human);
                return;
            }
            human.airSpeed -= human.airSpeedC*0.8f;
            human.nextPos.x = -human.currentDelta * human.diagonalSpeed * (human.airSpeed / human.weight);
            human.nextPos.y = human.nextPos.x;
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.transform.position = new Vector3(human.transform.position.x + human.nextPos.x,
                human.transform.position.y + human.nextPos.y,
                human.transform.position.z);
        }
        public void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.UPHILL;
        }
    }
    private class GroundUpHillForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.airSpeed = 0f;
            if (human.moveInput.input.Count > 0)
            {
                if (human.moveInput.input[0] == MoveInput.Left)
                {
                    human.moveInput.checkInput = human.CheckStartZ;
                    human.moveInput.actionInput = human.ActionStartZ;
                }
                else
                {
                    human.moveInput.checkInput = human.CheckStartE;
                    human.moveInput.actionInput = human.ActionStartE;
                }
            }
            else
            {
                human.moveInput.checkInput = human.CheckStand;
                human.moveInput.actionInput = human.ActionStand;
            }
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    Human.emptyFailStartForm.UpdateInit(human);
                    human.humanUpdateForm = Human.emptyFailStartForm;
                }
                else if(eIForm == EInputForm.FLAT)
                {
                    human.humanUpdateForm = Human.groundFlatForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.DOWNHILL)
                {
                    human.humanUpdateForm = Human.groundDownHillForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if (Input.GetKey(KeyCode.J))
            {
                human.beforeUpdateForm = Human.groundUpHillForm;
                human.humanUpdateForm = Human.jumpReadyForm;
                human.humanUpdateForm.UpdateInit(human);
                //human.humanUpdateForm.HumanUpdate(human);
                return;
            }
            Human.UpdateMoveInputMacro(human, KeyCode.A, MoveInput.Left, ref human.moveInput.pressLeft, human.CheckStartZ, human.CheckStand, human.CheckStartE);
            Human.UpdateMoveInputMacro(human, KeyCode.D, MoveInput.Right, ref human.moveInput.pressRight, human.CheckStartE, human.CheckStand, human.CheckStartZ);
            human.moveInput.checkInput();
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.moveInput.actionInput();
        }
        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.UPHILL;
        }
    }
    private class GroundDownHillForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.airSpeed = 0f;
            human.humanContact = Human.groundLandContact;
            if (human.moveInput.input.Count > 0)
            {
                if (human.moveInput.input[0] == MoveInput.Left)
                {
                    human.moveInput.checkInput = human.CheckStartQ;
                    human.moveInput.actionInput = human.ActionStartQ;
                }
                else
                {
                    human.moveInput.checkInput = human.CheckStartC;
                    human.moveInput.actionInput = human.ActionStartC;
                }
            }
            else
            {
                human.moveInput.checkInput = human.CheckStand;
                human.moveInput.actionInput = human.ActionStand;
            }
        }
        public virtual void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    Human.emptyFailStartForm.UpdateInit(human);
                    human.humanUpdateForm = Human.emptyFailStartForm;
                }
                else if(eIForm == EInputForm.FLAT)
                {
                    human.humanUpdateForm = Human.groundFlatForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if(eIForm == EInputForm.UPHILL)
                {
                    human.humanUpdateForm = Human.groundUpHillForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if (Input.GetKey(KeyCode.J))
            {
                human.beforeUpdateForm = Human.groundDownHillForm;
                human.humanUpdateForm = Human.jumpReadyForm;
                human.humanUpdateForm.UpdateInit(human);
                //human.humanUpdateForm.HumanUpdate(human);
                return;
            }
            Human.UpdateMoveInputMacro(human, KeyCode.A, MoveInput.Left, ref human.moveInput.pressLeft, human.CheckStartQ, human.CheckStand, human.CheckStartC);
            Human.UpdateMoveInputMacro(human, KeyCode.D, MoveInput.Right, ref human.moveInput.pressRight, human.CheckStartC, human.CheckStand, human.CheckStartQ);
            human.moveInput.checkInput();
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.moveInput.actionInput();
        }
        public virtual void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.DOWNHILL;
        }
    }
    private class GroundDownHillLandForm : IHumanUpdateForm
    {
        public void UpdateInit(Human human)
        {
            human.humanContact = Human.groundLandContact;
            if (human.objectSprite.flipX)
            {
                human.objectSprite.sprite = human.stairAnimation[0];
            }
            else
            {
                human.objectSprite.sprite = human.stairAnimation[1];
            }
        }
        public void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if (eIForm == EInputForm.FAIL)
                {
                    human.humanUpdateForm = Human.emptyFailStartForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
                else if (eIForm == EInputForm.FLAT)
                {
                    human.humanUpdateForm = Human.groundFlatLandForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public void HumanUpdate(Human human)
        {
            if (human.airSpeed < 0f)
            {
                human.airSpeed = 0f;
                human.humanUpdateForm = Human.groundUpHillForm;
                human.humanUpdateForm.UpdateInit(human);
                return;
            }
            human.airSpeed -= human.airSpeedC*0.8f;
            human.nextPos.x = human.currentDelta * human.diagonalSpeed * (human.airSpeed / human.weight);
            human.nextPos.y = -human.nextPos.x;
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x + human.nextPos.x, human.transform.position.y - halfSize.y + human.nextPos.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x + human.nextPos.x, human.transform.position.y + halfSize.y + human.nextPos.y);
            human.map.CrushCheck(in min, in max, human);
            human.transform.position = new Vector3(human.transform.position.x + human.nextPos.x,
                human.transform.position.y + human.nextPos.y,
                human.transform.position.z);
        }
        public void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.UPHILL;
        }
    }
    protected void CheckStartA()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnLeft, ActionStartA);
    }
    protected void CheckStartD()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnRight, ActionStartD);
    }
    protected void CheckStartE()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnRight, ActionStartE);
    }
    protected void CheckStartZ()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnLeft, ActionStartZ);
    }
    protected void CheckStartQ()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnLeft, ActionStartQ);
    }
    protected void CheckStartC()
    {
        nextPos = Vector2.zero;
        CheckStartMacro(TurnRight, ActionStartC);
    }
    protected void CheckStartMacro(Action turnDir, Action startAction)
    {
        turnDir();
        moveInput.checkInput = NullFunc;
        moveInput.actionInput = startAction;
    }
    protected void CheckRunA()
    {
        nextPos.x = -(currentDelta * speed);
        nextPos.y = 0f;
    }
    protected void CheckRunD()
    {
        nextPos.x = currentDelta * speed;
        nextPos.y = 0f;
    }
    protected void CheckRunE()
    {
        nextPos.x = currentDelta * diagonalSpeed;
        nextPos.y = nextPos.x;
        moveInput.actionInput = ActionRunEQ;
    }
    protected void CheckRunZ()
    {
        nextPos.x = -(currentDelta * diagonalSpeed);
        nextPos.y = nextPos.x;
        moveInput.actionInput = ActionRunZC;
    }
    protected void CheckRunQ()
    {
        nextPos.y = currentDelta * diagonalSpeed;
        nextPos.x = -nextPos.y;
        moveInput.actionInput = ActionRunEQ;
    }
    protected void CheckRunC()
    {
        nextPos.x = currentDelta * diagonalSpeed;
        nextPos.y = -nextPos.x;
        moveInput.actionInput = ActionRunZC;
    }
    protected void ActionStand()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + nextPos.x, gameObject.transform.position.y, 0f);
        StandAnimation();
    }
    protected void ActionStartA()
    {
        ActionStartMacro(1, MoveAnimation, ActionRunAD, CheckRunA);
    }

    protected void ActionStartD()
    {
        ActionStartMacro(1, MoveAnimation, ActionRunAD, CheckRunD);
    }
    protected void ActionStartE()
    {
        ActionStartMacro(0, StairUpAnimation, ActionRunEQ, CheckRunE);
    }
    protected void ActionStartQ()
    {
        ActionStartMacro(0, StairUpAnimation, ActionRunEQ, CheckRunQ);
    }
    protected void ActionStartZ()
    {
        ActionStartMacro(1, StairDownAnimation, ActionRunZC, CheckRunZ);
    }
    protected void ActionStartC()
    {
        ActionStartMacro(1, StairDownAnimation, ActionRunZC, CheckRunC);
    }
    protected void ActionStartMacro(int animationIndxInit, Action animation, Action moveAction, Action moveCheck)
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + nextPos.x, gameObject.transform.position.y, 0f);
        animationIndx = animationIndxInit;
        delay = frame;
        animation();
        moveInput.checkInput = moveCheck;
        moveInput.actionInput = moveAction;
    }

    protected void ActionRunAD()
    {
        ActionRunMacro(MoveAnimation);
    }
    protected void ActionRunEQ()
    {
        ActionRunMacro(StairUpAnimation);
    }
    protected void ActionRunZC()
    {
        ActionRunMacro(StairDownAnimation);
    }
    protected void ActionRunMacro(Action animation)
    {
        transform.position = new Vector3(transform.position.x + nextPos.x, transform.position.y + nextPos.y, transform.position.z);
        animation();
    }


    //Top은 그 키가 눌렀을 경우에 액션, Bottom은 그 반대의 키가 눌렀을 경우에 액션
    private static void UpdateMoveInputMacro(Human human, KeyCode key, int keyValue, ref bool keyPress, Action topAction, Action middleAction, Action bottomAction)
    {  
        if (Input.GetKey(key) && (!keyPress))
        {
            human.moveInput.input.Insert(0, keyValue);
            human.moveInput.checkInput = topAction;
            keyPress = true;
        }
        else if ((!Input.GetKey(key)) && keyPress)
        {
            for (int i = 0; i < human.moveInput.input.Count; i++)
            {
                if (human.moveInput.input[i] == keyValue)
                {
                    human.moveInput.input.RemoveAt(i);
                    break;
                }
            }
            if (human.moveInput.input.Count > 0) human.moveInput.checkInput = bottomAction;
            else human.moveInput.checkInput = middleAction;
            keyPress = false;
        }
    }

    private class RailTouchStandForm : IHumanUpdateForm
    {
        public virtual void UpdateInit(Human human)
        {
            human.objectSprite.sprite = human.railTouchedAnimation[0];
            human.moveInput.checkInput = human.NullFunc;
            human.moveInput.actionInput = human.NullFunc;
            human.airSpeed = 0f;
            human.airSpeedX = human.speed;
        }
        public void ChangeUpdateForm(EBackGroundForm eBGForm, EInputForm eIForm, Human human)
        {
            if (eBGForm == EBackGroundForm.GROUND)
            {
                if(eIForm == EInputForm.FAIL)
                {
                    human.humanUpdateForm = Human.emptyFailStartForm;
                    human.humanUpdateForm.UpdateInit(human);
                }
            }
        }
        public virtual void HumanUpdate(Human human)
        {
            if (Input.GetKey(KeyCode.J))
            {
                human.humanUpdateForm = Human.railTouchJumpReadyForm;
                human.humanUpdateForm.UpdateInit(human);
            }
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x, human.transform.position.y - halfSize.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x, human.transform.position.y + halfSize.y);
            human.map.CrushCheck(in min, in max, human);
        }
        public void GetUpdateForm(Human human, out EBackGroundForm eBGForm, out EInputForm eIForm)
        {
            eBGForm = EBackGroundForm.GROUND;
            eIForm = EInputForm.RailTouch;
        }
    }

    private class RailTouchJumpReadyForm : RailTouchStandForm
    {
        public override void UpdateInit(Human human)
        {
            human.objectSprite.sprite = human.railTouchedAnimation[1];
        }
        public override void HumanUpdate(Human human)
        {
            if (human.airSpeed > human.weight * 0.8f)
            {
                human.airSpeed = human.weight * 0.8f;
                human.humanUpdateForm = Human.jumpRunForm;
                human.humanUpdateForm.UpdateInit(human);
            }
            if (!Input.GetKey(KeyCode.J))
            {
                human.humanUpdateForm = Human.jumpRunForm;
                human.humanUpdateForm.UpdateInit(human);
            }
            human.airSpeed += human.airSpeedC * 0.5f;
            Vector2 halfSize = human.objectSprite.size * 0.5f;
            Vector2 min = new Vector2(human.transform.position.x - halfSize.x, human.transform.position.y - halfSize.y);
            Vector2 max = new Vector2(human.transform.position.x + halfSize.x, human.transform.position.y + halfSize.y);
            human.map.CrushCheck(in min, in max, human);
        }
    }

    //이동키 입력에 관한 변수
    protected class MoveInput
    {
        public List<int> input = new List<int>(2);
        public const int Right = 6;
        public const int Left = 4;
        public Action checkInput;
        public Action actionInput;

        //Input.GetKeyUp에서 오류가 종종 나서(예를 들면 키를 땠는데 GetKeyUp이 true를 호출하지 않아서 락이 걸린 현상이 나타남. 그래서 수정함)
        //위와 같은 이유로 추가된 변수
        public bool pressLeft = false;
        public bool pressRight = false;
    }
    protected MoveInput moveInput;

    protected interface IHumenContact
    {
        void ContactNull(Human human, in float posX, in float posY, in float size);
        void ContactG00(Human human, in float posX, in float posY, in float size);
        void ContactG01(Human human, in float posX, in float posY, in float size);
        void ContactG10(Human human, in float posX, in float posY, in float size);
        void ContactG11(Human human, in float posX, in float posY, in float size);
        void ContactG00G01(Human human, in float posX, in float posY, in float size);
        void ContactG01G11(Human human, in float posX, in float posY, in float size);
        void ContactG00G10(Human human, in float posX, in float posY, in float size);
        void ContactG10G11(Human human, in float posX, in float posY, in float size);
        void ContactG00G11(Human human, in float posX, in float posY, in float size);
        void ContactG01G10(Human human, in float posX, in float posY, in float size);
        void ContactG00G01G11(Human human, in float posX, in float posY, in float size);
        void ContactG00G01G10(Human human, in float posX, in float posY, in float size);
        void ContactG00G10G11(Human human, in float posX, in float posY, in float size);
        void ContactG01G10G11(Human human, in float posX, in float posY, in float size);
    }
    protected IHumenContact humanContact;

    protected class EmptyContact : IHumenContact
    {
        public virtual void ContactNull(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG01(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG10(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G01(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG01G11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G10(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG10G11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG01G10(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G01G11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G01G10(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG00G10G11(Human human, in float posX, in float posY, in float size) { }
        public virtual void ContactG01G10G11(Human human, in float posX, in float posY, in float size) { }
    }

    protected class GroundLandContact : EmptyContact
    {
        public override void ContactNull(Human human, in float posX, in float posY, in float size)
        {
            Vector2 foot = new Vector2(human.transform.position.x, human.transform.position.y - (human.objectSprite.size.y * 0.5f));
            float halfSize = size * 0.5f;
            if (foot.x > posX - halfSize &&
                foot.x < posX + halfSize &&
                foot.y > posY - halfSize &&
                foot.y < posY + halfSize)
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
        }
        public override void ContactG00(Human human, in float posX, in float posY, in float size)
        {
            //human.GroundLandUpDownHillMacro(in posX, in posY, in size, -1f, -1f, EInputForm.DOWNHILL);
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY = nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                float answerY = -(nextPos.x - (posX - half)) + posY + humanSize.y * 0.5f;
                if (!human.objectSprite.flipX)
                {
                    answerY += 1f;
                }
                human.nextPos.y = answerY - human.transform.position.y;

                EBackGroundForm eBGForm;
                EInputForm eIForm;
                human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
                if (eIForm != EInputForm.DOWNHILL)
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.DOWNHILL, human);
            }
        }
        public override void ContactG01(Human human, in float posX, in float posY, in float size)
        {
            //human.GroundLandUpDownHillMacro(in posX, in posY, in size, 1f, 1f, EInputForm.UPHILL);
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY = nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                float answerY = (nextPos.x - (posX + half)) + posY + humanSize.y * 0.5f;
                if (human.objectSprite.flipX)
                {
                    answerY += 1f;
                }
                human.nextPos.y = answerY - human.transform.position.y;

                EBackGroundForm eBGForm;
                EInputForm eIForm;
                human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
                if (eIForm != EInputForm.UPHILL)
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.UPHILL, human);
            }
        }
        public override void ContactG10(Human human, in float posX, in float posY, in float size)
        {
            Vector2 foot = new Vector2(human.transform.position.x, human.transform.position.y - (human.objectSprite.size.y * 0.5f));
            float halfSize = size * 0.5f;
            if (foot.x > posX - halfSize &&
                foot.x < posX + halfSize &&
                foot.y > posY - halfSize &&
                foot.y < posY + halfSize)
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
        }
        public override void ContactG11(Human human, in float posX, in float posY, in float size)
        {
            Vector2 foot = new Vector2(human.transform.position.x, human.transform.position.y - (human.objectSprite.size.y * 0.5f));
            float halfSize = size * 0.5f;
            if (foot.x > posX - halfSize &&
                foot.x < posX + halfSize &&
                foot.y > posY - halfSize &&
                foot.y < posY + halfSize)
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
        }
        public override void ContactG00G01(Human human, in float posX, in float posY, in float size)
        {
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY = nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                human.nextPos.y = posY + humanSize.y * 0.5f - human.gameObject.transform.position.y;
                EBackGroundForm eBGForm;
                EInputForm eIForm;
                human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
                if (eIForm != EInputForm.FLAT)
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FLAT, human);
            }
        }
        public override void ContactG00G11(Human human, in float posX, in float posY, in float size)
        {
            if (human.transform.position.x > posX)
            {
                ContactG00G10G11(human, in posX, in posY, in size);
            }
            else
            {
                ContactG00G01G11(human, in posX, in posY, in size);
            }
        }
        public override void ContactG01G10(Human human, in float posX, in float posY, in float size)
        {
            if (human.transform.position.x > posX)
            {
                ContactG00G01G10(human, in posX, in posY, in size);
            }
            else
            {
                ContactG01G10G11(human, in posX, in posY, in size);
            }
        }
        public override void ContactG00G10(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            x = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.25f;
            if (x > posX - halfSize &&
                x < posX)
            {
                human.nextPos.x = 0f;
                human.moveInput.actionInput = human.ActionStand;
            }
            //float humanHalfHeight = human.gameObject.GetComponent<SpriteRenderer>().size.y * 0.5f;
            float bottomY = human.gameObject.transform.position.y + human.nextPos.y - human.objectSprite.size.y * 0.5f;
            if (bottomY < posY + halfSize &&
                bottomY > posY)
            {
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
            }
        }

        public override void ContactG01G11(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            x = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.25f;
            if (x > posX &&
                x < posX + halfSize)
            {
                human.nextPos.x = 0f;
                human.moveInput.actionInput = human.ActionStand;
            }
            //float humanHalfHeight = human.gameObject.GetComponent<SpriteRenderer>().size.y * 0.5f;
            float bottomY = human.gameObject.transform.position.y + human.nextPos.y - human.gameObject.GetComponent<SpriteRenderer>().size.y * 0.5f;
            if (bottomY < posY + halfSize &&
                bottomY > posY)
            {
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
            }
        }
        public override void ContactG00G01G11(Human human, in float posX, in float posY, in float size)
        {
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY= nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                float answerY = (nextPos.x - (posX + -1f* half)) + posY + humanSize.y * 0.5f;
                if (human.objectSprite.flipX)
                {
                    answerY += 1f;
                }
                human.nextPos.y = answerY - human.transform.position.y;

                EBackGroundForm eBGForm;
                EInputForm eIForm;
                human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
                if (eIForm != EInputForm.UPHILL)
                {
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.UPHILL, human);
                }
            }
        }
        public override void ContactG00G01G10(Human human, in float posX, in float posY, in float size)
        {
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY = nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                float answerY = -(nextPos.x - (posX + half)) + posY + humanSize.y * 0.5f;
                if (!human.objectSprite.flipX)
                {
                    answerY += 1f;
                }
                human.nextPos.y = answerY - human.transform.position.y;

                EBackGroundForm eBGForm;
                EInputForm eIForm;
                human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
                if (eIForm != EInputForm.DOWNHILL)
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.DOWNHILL, human);
            }
        }

        public override void ContactG00G10G11(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y = human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            if (human.objectSprite.flipX)
            {
                head.x = human.transform.position.x + human.nextPos.x - 1f;
            }
            else
            {
                head.x = human.transform.position.x + human.nextPos.x;
            }
            if (head.x > posX - sizeHalf&&
                head.x < posX + sizeHalf &&
                head.y > posY - sizeHalf&&
                head.y < posY + sizeHalf)
            {
                float c = -posX - sizeHalf + posY;
                float answerY = head.x + c;
                if (head.y > answerY)
                {
                    float back = Mathf.Abs(head.x - head.y + c) / Mathf.Sqrt(2f);
                    human.nextPos.x += back;
                    human.nextPos.y -= back;
                    human.moveInput.actionInput = human.ActionStand;
                }
            }
        }
        public override void ContactG01G10G11(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y = human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            if (human.objectSprite.flipX)
            {
                head.x = human.transform.position.x + human.nextPos.x;
            }
            else
            {
                head.x = human.transform.position.x + human.nextPos.x + 1f;
            }
            if (head.x > posX - sizeHalf &&
                head.x < posX + sizeHalf &&
                head.y > posY - sizeHalf &&
                head.y < posY + sizeHalf)
            {
                float c = posX - sizeHalf + posY;
                float answerY = -head.x + c;
                if (head.y > answerY)
                {
                    float back = Mathf.Abs(-head.x - head.y + c) / Mathf.Sqrt(2f);
                    human.nextPos.x -= back;
                    human.nextPos.y -= back;
                    human.moveInput.actionInput = human.ActionStand;
                }
            }
        }
    }
    protected class GroundAirContact : EmptyContact
    {
        public override void ContactG00(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            if (human.transform.position.y - human.objectSprite.size.y * 0.5f < posY - halfSize)
            {
                float x = human.transform.position.x + human.nextPos.x;
                if (human.objectSprite.flipX)
                {
                    x -= 1f;
                }
                if (x < posX)
                    human.nextPos.x = posX - x;
                return;
            }
            human.GroundAirUpDownHillMacro(in posX, in posY, in size,-1f,-1f,EInputForm.DOWNHILL);
        }
        public override void ContactG01(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            if (human.transform.position.y - human.objectSprite.size.y * 0.5f < posY - halfSize)
            {
                float x = human.transform.position.x + human.nextPos.x;
                if (!human.objectSprite.flipX)
                {
                    x += 1f;
                }
                if (x > posX)
                    human.nextPos.x = posX - x;
                return;
            }
            human.GroundAirUpDownHillMacro(in posX, in posY, in size, 1f, 1f, EInputForm.UPHILL);
        }
        public override void ContactG10(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y= human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            head.x = human.transform.position.x + human.nextPos.x;
            float nextX = head.x;
            if (human.objectSprite.flipX)
            {
                nextX -= 1f;
            }

            if (nextX < posX &&
                posX-sizeHalf < head.x &&
                head.y > posY &&
                head.y < posY + sizeHalf)
            {
                float answerY = (nextX - (posX - sizeHalf)) + posY;
                if (head.y > answerY)
                {
                    human.nextPos.y = answerY - human.transform.position.y-human.objectSprite.size.y*0.5f;
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
                }
            }
        }
        public override void ContactG11(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y = human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            head.x = human.transform.position.x + human.nextPos.x;
            float nextX = head.x;
            if (!human.objectSprite.flipX)
            {
                nextX += 1f;
            }
            if (head.x < posX + sizeHalf &&
                posX < nextX &&
                head.y > posY &&
                head.y < posY + sizeHalf)
            {
                float answerY = -(nextX - (posX + sizeHalf)) + posY;
                if (head.y > answerY)
                {
                    human.nextPos.y = answerY - human.transform.position.y - human.objectSprite.size.y * 0.5f;
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
                }
            }
        }
        public override void ContactG00G01(Human human, in float posX, in float posY, in float size)
        {
            Vector2 nextPos = new Vector2(human.gameObject.transform.position.x + human.nextPos.x, human.gameObject.transform.position.y + human.nextPos.y);
            Vector2 humanSize = human.gameObject.GetComponent<SpriteRenderer>().size;
            float half = size * 0.5f;
            float footY = nextPos.y - humanSize.y * 0.5f;
            if (nextPos.x > posX - half &&
                nextPos.x < posX + half &&
                footY > posY - half &&
                footY < posY + half)
            {
                float answerY = posY + humanSize.y * 0.5f;
                if (answerY > nextPos.y)
                {
                    human.nextPos.y = answerY - human.gameObject.transform.position.y;
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FLAT, human);
                }
            }
        }
        public override void ContactG00G10(Human human, in float posX, in float posY, in float size)
        {
            //float halfSize = size * 0.5f;
            //float x = 0;
            //if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
            //    x -= 1f;
            //else
            //    x += 1f;
            //x = (human.gameObject.transform.position.x+human.nextPos.x) + x * human.objectSprite.size.x * 0.5f;
            //if (x > posX - halfSize &&
            //    x < posX) 
            //{
            //    human.nextPos.x = posX - x;
            //}
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            float xDict = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.5f;
            if (xDict > posX - halfSize &&
                xDict < posX)
            {
                xDict = human.gameObject.transform.position.x + x * human.objectSprite.size.x * 0.5f;
                if (xDict > posX - halfSize &&
                    xDict < posX)
                    human.nextPos.x = 1f;
                else human.nextPos.x = 0f;
            }
        }
        public override void ContactG01G11(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            float xDict = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.5f;
            if (xDict > posX &&
                xDict < posX + halfSize)
            {
                xDict = human.gameObject.transform.position.x + x * human.objectSprite.size.x * 0.5f;
                if (xDict > posX &&
                    xDict < posX + halfSize)
                    human.nextPos.x = -1f;
                else human.nextPos.x = 0f;
            }
        }
        public override void ContactG10G11(Human human, in float posX, in float posY, in float size) 
        {
            float head = human.transform.position.y + human.objectSprite.size.y * 0.5f + human.nextPos.y;
            float sizeHalf = size * 0.5f;
            if (head > posY &&
                head < posY + sizeHalf)
            {
                human.nextPos.y = posY - head;
                human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
            }
        }
        public override void ContactG00G11(Human human, in float posX, in float posY, in float size)
        {
            if (human.nextPos.y < 0)
            {
                ContactG00G01G11(human, in posX, in posY, in size);
            }
            else
            {
                ContactG00G10G11(human, in posX, in posY, in size);
            }
        }
        public override void ContactG01G10(Human human, in float posX, in float posY, in float size)
        {
            if (human.nextPos.y < 0)
            {
                ContactG00G01G10(human, in posX, in posY, in size);
            }
            else
            {
                ContactG01G10G11(human, in posX, in posY, in size);
            }
        }
        public override void ContactG00G01G11(Human human, in float posX, in float posY, in float size)
        {
            human.GroundAirUpDownHillMacro(in posX, in posY, in size, 1f, -1f, EInputForm.UPHILL);
        }
        public override void ContactG00G01G10(Human human, in float posX, in float posY, in float size)
        {
            human.GroundAirUpDownHillMacro(in posX, in posY, in size, -1f, 1f, EInputForm.DOWNHILL);
        }
        public override void ContactG00G10G11(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y = human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            if (human.objectSprite.flipX)
            {
                head.x = human.transform.position.x + human.nextPos.x - 1f;
            }
            else
            {
                head.x = human.transform.position.x + human.nextPos.x;
            }
            if (head.x > posX - sizeHalf &&
                head.x < posX + sizeHalf &&
                head.y > posY - sizeHalf &&
                head.y < posY + sizeHalf)
            {
                float answerY = (head.x - (posX + sizeHalf)) + posY;
                if (head.y > answerY)
                {
                    human.nextPos.y = answerY - human.transform.position.y - human.objectSprite.size.y * 0.5f;
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
                }
            }
        }
        public override void ContactG01G10G11(Human human, in float posX, in float posY, in float size)
        {
            float sizeHalf = size * 0.5f;
            Vector2 head;
            head.y = human.transform.position.y + human.nextPos.y + human.objectSprite.size.y * 0.5f;
            if (human.objectSprite.flipX)
            {
                head.x = human.transform.position.x + human.nextPos.x;
            }
            else
            {
                head.x = human.transform.position.x + human.nextPos.x + 1f;
            }
            if (head.x > posX - sizeHalf &&
                head.x < posX + sizeHalf &&
                head.y > posY - sizeHalf &&
                head.y < posY + sizeHalf)
            {
                float answerY = -(head.x - (posX - sizeHalf)) + posY;
                if (head.y > answerY)
                {
                    human.nextPos.y = answerY - human.transform.position.y - human.objectSprite.size.y * 0.5f;
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.FAIL, human);
                }
            }
        }
    }
    protected class GroundRailTouchContact : GroundAirContact
    {
        public override void ContactG00(Human human, in float posX, in float posY, in float size)
        {
            EBackGroundForm eBGForm;
            EInputForm eIForm;
            human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
            if (eIForm != EInputForm.RailTouch)
            {
                base.ContactG00(human, in posX, in posY, in size);
            }
            else
            {
                human.transform.position = new Vector3(human.transform.position.x, human.transform.position.y - (human.currentDelta * human.weight), human.transform.position.z);
            }
        }
        public override void ContactG01(Human human, in float posX, in float posY, in float size)
        {
            EBackGroundForm eBGForm;
            EInputForm eIForm;
            human.humanUpdateForm.GetUpdateForm(human, out eBGForm, out eIForm);
            if (eIForm != EInputForm.RailTouch)
            {
                base.ContactG01(human, in posX, in posY, in size);
            }
            else
            {
                human.transform.position = new Vector3(human.transform.position.x, human.transform.position.y - (human.currentDelta * human.weight), human.transform.position.z);
            }
        }
        public override void ContactG00G10(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            float xDict = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.5f;
            if (xDict > posX - halfSize &&
                xDict < posX)
            {
                xDict = human.gameObject.transform.position.x + x * human.objectSprite.size.x * 0.5f;
                if (xDict > posX - halfSize &&
                    xDict < posX)
                    human.nextPos.x = 1f;
                else human.nextPos.x = 0f;
                if (human.moveInput.input.Count > 0 && human.moveInput.input[0] == MoveInput.Left)
                {
                    //Rail 업데이트로 바꿈
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.RailTouch, human);
                }
            }
        }
        public override void ContactG01G11(Human human, in float posX, in float posY, in float size)
        {
            float halfSize = size * 0.5f;
            float x = 0;
            if (human.gameObject.GetComponent<SpriteRenderer>().flipX)
                x -= 1f;
            else
                x += 1f;
            float xDict = (human.gameObject.transform.position.x + human.nextPos.x) + x * human.objectSprite.size.x * 0.5f;
            if (xDict > posX &&
                xDict < posX + halfSize)
            {
                xDict = human.gameObject.transform.position.x + x * human.objectSprite.size.x * 0.5f;
                if (xDict > posX &&
                    xDict < posX + halfSize)
                    human.nextPos.x = -1f;
                else human.nextPos.x = 0f;
                if (human.moveInput.input.Count > 0 && human.moveInput.input[0] == MoveInput.Right)
                {
                    //Rail 업데이트로 바꿈
                    human.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, EInputForm.RailTouch, human);
                }
            }
        }
    }

    public void EmptyGround(in float posX, in float posY, in float size)
    {
        this.humanContact.ContactNull(this, in posX, in posY, in size);
    }
    public void EmptyWater()
    {

    }
    public virtual void ContactG00(float posX, float posY, float size)
    {
        //UpDownHillMacro(in posX, in posY, in size, -1f, -1f, EInputForm.DOWNHILL);
        this.humanContact.ContactG00(this, in posX, in posY, in size);
    }
    public virtual void ContactG01(float posX, float posY, float size)
    {
        //UpDownHillMacro(in posX, in posY, in size, 1f, 1f, EInputForm.UPHILL);
        this.humanContact.ContactG01(this, in posX, in posY, in size);
    }

    private void GroundAirUpDownHillMacro(in float posX, in float posY, in float size, float m, float halfSign, EInputForm form)
    {
        Vector2 nextPos = new Vector2(this.gameObject.transform.position.x + this.nextPos.x, this.gameObject.transform.position.y + this.nextPos.y);
        Vector2 humanSize = this.gameObject.GetComponent<SpriteRenderer>().size;
        float half = size * 0.5f;
        float footY = nextPos.y - humanSize.y * 0.5f;
        if (nextPos.x > posX - half &&
            nextPos.x < posX + half &&
            footY > posY - half &&
            footY < posY + half)
        {
            float answerY = m * (nextPos.x - (posX + halfSign * half)) + posY + humanSize.y * 0.5f;
            if (nextPos.y < answerY)
            {
                this.nextPos.y = answerY - this.gameObject.transform.position.y;
                this.humanUpdateForm.ChangeUpdateForm(EBackGroundForm.GROUND, form, this);
            }
        }
    }
    public virtual void ContactG11(float posX, float posY, float size)
    {
        humanContact.ContactG11(this, in posX, in posY, in size);
    }
    public virtual void ContactG10(float posX, float posY, float size)
    {
        humanContact.ContactG10(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G01(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G01(this, in posX, in posY, in size);
    }
    public virtual void ContactG01G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG01G11(this, in posX, in posY, in size);
    }

    public virtual void ContactG10G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG10G11(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G10(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G10(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G11(this, in posX, in posY, in size);
    }
    public virtual void ContactG01G10(float posX, float posY, float size)
    {
        this.humanContact.ContactG01G10(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G01G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G01G11(this, in posX, in posY, in size);
    }
    public virtual void ContactG01G10G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG01G10G11(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G10G11(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G10G11(this, in posX, in posY, in size);
    }
    public virtual void ContactG00G01G10(float posX, float posY, float size)
    {
        this.humanContact.ContactG00G01G10(this, in posX, in posY, in size);
    }
    public virtual void ContactGALL(float posX, float posY, float size)
    {

    }

    public virtual void ContactW00(float posX, float posY, float size)
    {

    }

    public virtual void ContactW01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W10W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W10W11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01W10(float posX, float posY, float size)
    {

    }
    public virtual void ContactWALL(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW11G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10G00(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01G00(float posX, float posY, float size)
    {

    }
    public virtual void ContactW11G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00G01G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01G10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW11G00G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10G00G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01G00G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW11G00G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10G01G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00G10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00G01G10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01G00G10G11(float posX, float posY, float sizee)
    {

    }
    public virtual void ContactW11G00G01G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10G00G01G11(float posX, float posY,float size)
    {

    }
    public virtual void ContactW01W10G00(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W11G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W11G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01G10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W11G00G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW10W11G00G01(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W10G01G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W11G01G10(float posX, float posY, float size)
    {

    }
    public virtual void ContactW01W10G00G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01W10G11(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W01W11G10(float posX, float posY, float sizee)
    {

    }
    public virtual void ContactW01W10W11G00(float posX, float posY, float size)
    {

    }
    public virtual void ContactW00W10W11G01(float posX, float posY, float size)
    {

    }
}


