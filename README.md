# Unity 3D Action Portfolio

플레이어 이동 및 점프, 물리 기반 컨트롤 시스템을 구현한 Unity 프로젝트입니다.

## 주요 기능

- Rigidbody 기반 이동 시스템
- 자연스러운 점프 구현 (중복 입력 방지)
- 공중 제어 (Air Control)
- 입력 시스템 분리 (InputController)
- 바닥 체크 (Raycast 기반)



- ## 사용 기술

- Unity 2022.3 LTS
- C#
- Rigidbody Physics



- ## 구현 상세

### 1. 이동 시스템
- Rigidbody velocity를 직접 제어하여 즉각적인 반응 구현

### 2. 점프 시스템
- GetKeyDown 기반 입력 처리
- 공중에서 중복 점프 방지

### 3. 바닥 체크
- Raycast를 활용한 Ground Detection

### 4. 공중 제어
- 공중에서 방향 전환 시 자연스러운 속도 보간 처리



- ## 트러블슈팅

### 문제 1: 점프 높이가 불안정함
- 원인: AddForce 중복 적용
- 해결: JumpPressed 소비 방식 적용

### 문제 2: 공중에서 이동이 부자연스러움
- 원인: velocity 강제 설정
- 해결: Lerp 기반 방향 전환 적용



## 실행 방법

1. Unity 2022.3 LTS에서 실행
2. MainScene 실행
