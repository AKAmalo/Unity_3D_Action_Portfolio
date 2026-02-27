# Unity 3D Action Controller

물리 기반 캐릭터 컨트롤과 공중 제어(Air Control)를 고려한  
3D 액션 이동 시스템 구현 프로젝트

---

## 🎮 Demo

> 캐릭터 이동, 점프, 공중 제어가 자연스럽게 동작하는 모습

![demo](링크)

---

## 🎮 Controls

- 이동: WASD
- 점프: Space

---

## 🎯 핵심 구현

- Rigidbody 기반 물리 이동 시스템 설계
- InputController를 통한 입력 / 로직 분리 구조 적용
- 공중 제어(Air Control)를 고려한 방향 보간 처리
- Raycast 기반 Ground Detection으로 점프 안정성 확보
- 카메라 기반 이동 시스템 (카메라 방향 기준 이동)
- 캐릭터 회전과 이동 방향 동기화
- Animator 기반 Idle / Walk 애니메이션 상태 전환

---

## 🧠 설계

### InputController 분리 이유
- 입력 처리와 캐릭터 로직을 분리하여 **책임 분리(SRP)** 적용
- 입력 방식 변경 (키보드 → AI / 네트워크 입력) 시  
  캐릭터 로직 수정 없이 확장 가능

### Rigidbody 기반 이동 선택 이유
- 물리 엔진을 활용한 자연스러운 가속/감속 구현
- CharacterController 대비 충돌 및 반응이 더 현실적
- FixedUpdate 기반 물리 처리로 일관된 동작 보장

### 카메라 기반 이동 설계
- 플레이어 입력을 월드 기준이 아닌 카메라 기준으로 변환
- 3인칭 시점에서 직관적인 조작 제공

---

## ⚙️ 구현 상세

### 이동 시스템
- Rigidbody.velocity 직접 제어로 즉각적인 입력 반응 확보
- 공중 상태에서는 이동 속도 제한 (Air Control 비율 적용)
- 카메라 forward/right 벡터를 활용한 방향 계산

### 회전 시스템
- 이동 방향 기준으로 캐릭터 회전
- Quaternion 기반 보간으로 부드러운 회전 처리

### 점프 시스템
- AddForce(Impulse) 기반 점프 적용
- 입력 상태를 1회 소비하는 구조로 중복 점프 방지

### 바닥 체크
- Raycast 기반 Ground Detection
- 일정 거리 이하일 때만 점프 가능하도록 제한

### 공중 제어 (Air Control)
- Lerp 기반 방향 전환
- 지상 대비 공중 이동 영향도 감소

### 애니메이션 시스템
- Animator Controller를 통한 상태 머신 구성
- Speed 파라미터 기반 Idle / Walk 전환
- 이동 속도에 따라 자연스럽게 애니메이션 변경

---

## 🚧 트러블슈팅

### 문제 1: 점프 높이가 프레임마다 달라짐
- 원인: AddForce가 Update에서 중복 적용됨
- 해결:
  - 입력을 1회만 처리하는 구조로 변경
  - 모든 물리 연산을 FixedUpdate로 이동

---

### 문제 2: 공중 이동이 부자연스러움
- 원인: velocity를 즉시 변경하여 방향 전환이 급격함
- 해결:
  - Lerp를 활용한 점진적 방향 전환
  - 공중 제어 비율을 적용하여 자연스러운 움직임 구현

---

### 문제 3: 캐릭터 회전이 끊기는 현상
- 원인: 회전을 즉시 적용하여 급격한 방향 전환 발생
- 해결:
  - Slerp 또는 Lerp를 활용한 회전 보간 적용

---

### 문제 4: 캐릭터 모델이 찌그러지는 현상
- 원인: 부모 오브젝트 스케일 또는 회전 구조 문제
- 해결:
  - ModelRoot 분리 후 회전은 ModelRoot에만 적용
  - 스케일은 (1,1,1) 유지

---

## 🛠 Tech Stack

- Unity 2022.3 LTS
- C#
- Rigidbody Physics
- Animator (Mecanim)

---

## ▶ 실행 방법

1. Unity 2022.3 LTS에서 프로젝트 실행
2. MainScene 실행
