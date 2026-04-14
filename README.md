# Unity 3D Action Controller

물리 기반 캐릭터 컨트롤과 공중 제어(Air Control)를 고려한
**3D 액션 캐릭터 이동 시스템 구현 프로젝트**

플레이어 FSM 기반 상태 관리와 카메라 피드백 시스템을 포함한
3인칭 액션 캐릭터 컨트롤러 구현.

---

# 🎮 Demo

> 캐릭터 이동, 점프, 낙하, 착지, 카메라 반응이 자연스럽게 동작하는 모습

![demo](링크)

---

# 🎮 Controls

| Action | Key   |
| ------ | ----- |
| Move   | WASD  |
| Run    | Shift |
| Jump   | Space |
| Camera | Mouse |

---

# 🎯 핵심 구현

### Player Controller

* Rigidbody 기반 물리 이동 시스템
* Player FSM 기반 상태 관리
* 공중 제어(Air Control) 이동 시스템
* 카메라 기준 이동 시스템

### Movement System

* Idle / Walk / Run 상태 전환
* 카메라 기준 방향 이동
* 이동 방향 기반 캐릭터 회전

### Jump & Fall System

* Rigidbody Impulse 기반 점프
* 공중 이동 제어(Air Control)
* Raycast 기반 Ground Detection

### Landing System

* 낙하 속도 기반 **Soft Landing / Hard Landing 분기**
* Hard Landing 시 **회전 제한**
* 착지 후 **이동 속도 점진적 회복**
* Landing 애니메이션 상태 관리

### Camera System

* 마우스 기반 카메라 회전
* 이동 방향 기반 **카메라 자동 정렬**
* 착지 시 **카메라 임팩트 및 흔들림 효과**

---

# 🧠 시스템 설계

## Player FSM (Finite State Machine)

플레이어 행동을 상태 단위로 분리하여 관리.

```
Idle
 ├ Move
 │   └ Run
 ├ Jump
 │   └ Fall
 │        └ Land
```

FSM 구조를 통해

* 상태 전환 관리
* 행동 로직 분리
* 시스템 확장 용이

---

## InputController 분리 이유

입력 처리와 캐릭터 로직을 분리하여 **책임 분리(SRP)** 적용

장점

* 입력 방식 교체 가능
  (Keyboard → AI → Network)

* 캐릭터 로직 독립 유지

---

## Rigidbody 기반 이동 선택 이유

* 물리 엔진을 활용한 자연스러운 움직임
* CharacterController 대비 충돌 반응이 현실적
* FixedUpdate 기반 물리 처리

---

## 카메라 기반 이동 설계

플레이어 입력을 **카메라 기준 방향**으로 변환

```
moveDir =
camera.forward * input.y +
camera.right * input.x
```

이를 통해 3인칭 액션 게임에서
직관적인 조작감을 제공.

---

# ⚙️ 구현 상세

## 이동 시스템

* Rigidbody.velocity 직접 제어
* 이동 입력 즉각 반응
* Run 상태 속도 증가

---

## 공중 제어 (Air Control)

공중 상태에서 이동 영향 감소

```
velocity.x = Mathf.Lerp(current, target, airControl)
velocity.z = Mathf.Lerp(current, target, airControl)
```

지상 대비 공중 이동 영향도를 줄여
더 자연스러운 움직임 구현.

---

## 회전 시스템

이동 방향 기준 캐릭터 회전

```
Quaternion.RotateTowards()
```

* 부드러운 방향 전환
* 입력 없는 상태에서 떨림 방지

---

## 점프 시스템

Impulse 기반 점프 적용

```
rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse)
```

* 점프 입력 1회 소비
* 중복 점프 방지

### ▶ 입력 보정 시스템 (추가 구현)

#### Coyote Time

* 낙하 직후 일정 시간 동안 점프 허용
* 플랫폼 끝에서 떨어진 직후에도 자연스럽게 점프 가능

#### Jump Buffer

* 점프 입력을 일정 시간 저장
* 착지 직전 입력을 놓치지 않도록 보정

#### 적용 결과

* 점프 입력 씹힘 현상 제거
* 입력 타이밍에 대한 스트레스 감소
* 액션 게임 수준의 입력 반응성 확보

---

## 낙하 시스템

점프 최고점 이후 Fall 상태 전환

```
if (rb.velocity.y <= 0)
    ChangeState(FallState)
```

---

## 착지 시스템

낙하 최고 속도를 기록하여
착지 강도를 판별.

```
if (maxFallSpeed < hardLandingThreshold)
    HardLanding
else
    SoftLanding
```

### Soft Landing

* 이동 가능
* 카메라 약한 임팩트

### Hard Landing

* 이동 제한
* 회전 제한
* 카메라 강한 임팩트
* 이동 속도 점진 회복

### ▶ 추가 개선 (이번 작업)

* Hard Landing 중 **점프 입력 차단**
* Jump Buffer가 Landing 상태를 무시하지 않도록 제어
* Landing 상태에서 점프 우선순위 조건 분기 처리

---

## 카메라 시스템

### 카메라 회전

마우스 입력 기반 회전

```
yaw += mouseX
pitch -= mouseY
```

---

### 카메라 자동 정렬

플레이어 이동 방향 기준 카메라 회전

조건

* 일정 시간 마우스 입력 없음
* 플레이어 이동 중

---

### 카메라 착지 임팩트

착지 시 카메라 위치에 임팩트 적용

구성

* 카메라 Drop (아래로 눌림)
* Camera Shake

Hard Landing 시 더 강하게 적용.

---

# 📂 Project Structure

```
Assets
 ├ Scripts
 │  ├ Player
 │  │  ├ PlayerMovement.cs
 │  │  ├ PlayerInputController.cs
 │  │
 │  │  ├ States
 │  │  │  ├ IdleState.cs
 │  │  │  ├ MoveState.cs
 │  │  │  ├ RunState.cs
 │  │  │  ├ JumpState.cs
 │  │  │  ├ FallState.cs
 │  │  │  └ LandState.cs
 │  │
 │  │  └ StateMachine
 │  │     ├ PlayerStateMachine.cs
 │  │     └ IPlayerState.cs
 │  │
 │  └ Camera
 │     └ CameraController.cs
```

플레이어 시스템을 FSM 구조로 설계하여
각 행동을 상태 단위로 분리하여 관리하였다.

---

# 🧩 System Architecture

플레이어 입력부터 캐릭터 행동까지의 흐름

```
Player Input
      │
      ▼
PlayerInputController
      │
      ▼
PlayerMovement
      │
      ▼
PlayerStateMachine
      │
      ▼
Player States
 ├ IdleState
 ├ MoveState
 ├ RunState
 ├ JumpState
 ├ FallState
 └ LandState
```

각 상태는 독립적인 클래스로 구현되어
상태별 행동 로직을 분리하였다.

---

# 🎥 Camera System Flow

카메라 시스템 구조

```
Mouse Input
      │
      ▼
CameraController
      │
      ├ Camera Rotation
      ├ Auto Camera Align
      └ Landing Camera Impact
```

카메라는 플레이어 이동 방향을 기반으로 자동 정렬되며
착지 시 카메라 임팩트 효과를 통해 액션 게임의 타격감을 강화하였다.

---

# 🚧 트러블슈팅

## 문제 1: 점프 높이가 프레임마다 달라짐

원인
AddForce가 Update에서 반복 실행

해결

* 입력을 1회만 소비
* 물리 연산을 FixedUpdate로 이동

---

## 문제 2: 공중 이동이 부자연스러움

원인
velocity 즉시 변경

해결

* Lerp 기반 방향 보간
* Air Control 비율 적용

---

## 문제 3: 캐릭터 회전이 끊김

원인
즉시 회전 적용

해결

* Quaternion.RotateTowards 적용

---

## 문제 4: Hard Landing 애니메이션 미작동

원인
Inspector 값 변경이 코드에 반영되지 않음

해결

* Landing Threshold 값 확인
* Animator 파라미터 점검

---

## 문제 5: 착지 임팩트 부자연스러움

원인
카메라 위치 즉시 변경

해결

* Drop + Shake 조합
* Lerp 기반 복원

---

## 문제 6: 점프 입력이 씹히는 문제 (추가)

원인
프레임 단위 입력 처리

해결

* Jump Buffer 적용
* 입력을 일정 시간 저장하도록 개선

---

## 문제 7: 낙하 직후 점프 불가 (추가)

원인
Ground 상태에 의존한 점프 조건

해결

* Coyote Time 적용
* 공중에서도 일정 시간 점프 허용

---

## 문제 8: Hard Landing이 점프로 취소되는 문제 (추가)

원인
Jump Buffer가 Landing 상태까지 유지됨

해결

* Hard Landing 상태에서 점프 입력 차단
* Jump Buffer 무효화 처리

---

# 🛠 Tech Stack

* Unity 2022.3 LTS
* C#
* Rigidbody Physics
* Animator (Mecanim)
* Finite State Machine

---

# ▶ 실행 방법

1. Unity 2022.3 LTS로 프로젝트 실행
2. `MainScene` 실행

---

# 📌 향후 확장 계획

* Variable Jump (점프 높이 제어)
* Fall Multiplier (낙하 가속)
* Landing Particle Effects
* Surface 기반 Footstep Sound
* Camera FOV Impact
* Combat System
