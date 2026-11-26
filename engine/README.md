# console-engine-coop
 
## 게임 소개
위에서 떨어지는 돌을 좌우로 피하며 60초를 버티는 콘솔 게임.
규칙과 화면은 [기획설계문서](../design/README.md)에 있습니다.
 
## 엔진 5분 시작 가이드
입력 · 갱신 · 렌더링만 하는 아주 작은 콘솔 게임 엔진입니다.
 
### 1. 엔진 연결
게임 프로젝트 우클릭 › 추가 › 프로젝트 참조 › ConsoleEngine 체크
 
### 2. GameBase 상속
GameBase를 상속하고 Input · Update · Render를 채웁니다.
```csharp
(탭 “예제: Program.cs”의 코드 전체)
```
 
### 3. 실행
Ctrl+F5 → ←/→로 @가 움직이고 Esc로 끝납니다.
 
## 더 알아보기
- 루프 순서·좌표·제약: docs/engine-guide.md
- 바뀐 점: CHANGELOG.md