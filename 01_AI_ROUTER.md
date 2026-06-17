# AI Router - Anima Spire 기준문서 라우터_v02

## 1. 문서 목적

이 문서는 Anima Spire 프로젝트의 AI 기준문서 진입점을 안내하는 라우터 문서다.

이 문서는 세부 판단기준, 출력기준, 구현 가드레일을 직접 정의하지 않는다.

이 문서는 전체 기준문서를 다시 읽게 만드는 목차가 아니라, 현재 요청에 필요한 기준문서와 세부 장 번호를 좁혀주는 색인 문서다.

AI는 현재 요청의 성격을 먼저 구분한 뒤, 이 문서에서 지정하는 기준문서와 세부 장을 우선 확인한다.

세부 판단기준은 `02_AI_WORKFLOW_RULES.md`를 따른다.

세부 출력기준은 `03_AI_OUTPUT_RULES.md`를 따른다.

구현자 또는 코드검토자 가드레일은 `IMPLEMENTATION_GUARDRAILS.md`를 따른다.

---

## 2. 기준문서 4종

Anima Spire 프로젝트의 소스 기준문서는 다음 4종으로 운영한다.

| 파일명                            | 역할                                   | 주 적용 대상             |
| ------------------------------ | ------------------------------------ | ------------------- |
| `01_AI_ROUTER.md`              | 기준문서 진입점 및 세부 장 번호 안내                | GPT, Claude, Gemini |
| `02_AI_WORKFLOW_RULES.md`      | 판단기준, 작업흐름, AI별 역할, 사용자 결정 기준        | GPT, Claude, Gemini |
| `03_AI_OUTPUT_RULES.md`        | 출력기준, 파일명, 문서블록, 코드블록, Drive 저장문서 형식 | GPT, Claude, Gemini |
| `IMPLEMENTATION_GUARDRAILS.md` | 구현자·코드검토자 가드레일                       | Codex, ClaudeCode   |

`01`, `02`, `03`은 AI가 순차 참조하는 문서에만 붙인다.

`IMPLEMENTATION_GUARDRAILS.md`는 구현자 가드레일로서 별도 병렬 트랙이므로 넘버링하지 않는다.

---

## 3. 기본 사용 순서

AI는 Anima Spire 관련 요청을 받으면 다음 순서로 기준문서를 적용한다.

1. `01_AI_ROUTER.md`에서 현재 요청의 성격을 확인한다.
2. 이 문서의 상황별 세부 라우팅표에서 우선 확인할 기준문서와 장 번호를 찾는다.
3. `02_AI_WORKFLOW_RULES.md`에서 현재 단계, 담당 AI, 사용자 결정 필요 여부를 판단한다.
4. 응답 또는 문서 출력이 필요한 경우 `03_AI_OUTPUT_RULES.md`에서 해당 출력형식을 적용한다.
5. 구현 또는 코드검토가 포함되는 경우 `IMPLEMENTATION_GUARDRAILS.md`의 금지사항과 완료보고 기준을 함께 적용한다.

AI는 모든 기준문서를 처음부터 끝까지 다시 훑는 것을 기본값으로 삼지 않는다.

먼저 이 문서에서 현재 요청 유형을 찾고, 지정된 문서와 장을 우선 확인한다.

단, 지정된 장만으로 판단이 불충분하거나 기준 충돌이 의심되는 경우에는 관련 문서의 상위 장 또는 전체 문서를 추가 확인한다.

기준문서 간 충돌이 발생하면 AI는 임의로 판단하지 않고 사용자에게 충돌 내용을 보고한 뒤 결정을 요청한다.

---

## 4. 상황별 세부 라우팅표

이 장은 현재 요청의 성격에 따라 확인해야 할 기준문서와 세부 장 번호를 안내한다.

AI는 현재 요청과 가장 가까운 상황을 먼저 찾고, 우선 확인 위치를 기준으로 판단한다.

함께 확인할 위치는 해당 작업에 출력, 구현, 검토, 저장, 테스트가 포함될 때 확인한다.

확인 생략 가능 항목은 해당 요청에 포함되지 않는 경우 생략할 수 있다.

| 현재 상황                                         | 우선 확인 위치                                   | 함께 확인할 위치                                                                             | 확인 생략 가능                                               |
| --------------------------------------------- | ------------------------------------------ | ------------------------------------------------------------------------------------- | ------------------------------------------------------ |
| 현재 요청이 어떤 작업인지 판단해야 함                         | `02_AI_WORKFLOW_RULES.md` 2장, 5장           | 필요 시 6장 또는 7장                                                                         | `03`, `IMPLEMENTATION`은 출력·구현이 없으면 생략                  |
| 기획, 방향, 로드맵, 큰 구조 논의                          | `02_AI_WORKFLOW_RULES.md` 6장               | Claude 필요 시 4장, 5장                                                                    | 구현이 없으면 `IMPLEMENTATION` 생략                            |
| Claude 기안문 작성 요청                              | `02_AI_WORKFLOW_RULES.md` 4장, 5장, 6장       | `03_AI_OUTPUT_RULES.md` 5장, 7.3장                                                      | 구현 가드레일 직접 검토는 보통 생략                                   |
| Claude 최종검토 필요 여부 판단                          | `02_AI_WORKFLOW_RULES.md` 11장              | `03_AI_OUTPUT_RULES.md` 9장, 11장                                                       | 구현이 없으면 `IMPLEMENTATION` 생략                            |
| Gemini 기술검토 요청문 작성                            | `02_AI_WORKFLOW_RULES.md` 5장, 7장           | `03_AI_OUTPUT_RULES.md` 3장, 5장, 7.1장, 8장 / 구현 구조, 회귀 위험, SaveData, Scene, UI, Android, Combat, Equipment, Legacy 관련 검토가 포함되면 `IMPLEMENTATION_GUARDRAILS.md` 관련 장 | 순수 기획·문서 구조 검토처럼 구현 가드레일과 직접 관련 없는 경우에만 `IMPLEMENTATION` 생략 가능 |
| Perplexity 검색요청문 작성                           | `02_AI_WORKFLOW_RULES.md` 5장               | `03_AI_OUTPUT_RULES.md` 3장, 5장, 7.2장, 8장                                              | `IMPLEMENTATION` 생략                                    |
| Codex 구현지시문 작성                                | `02_AI_WORKFLOW_RULES.md` 7장               | `03_AI_OUTPUT_RULES.md` 3장, 5장, 7.4장, 10장 / `IMPLEMENTATION_GUARDRAILS.md` 전체 또는 관련 장 | 생략 없음                                                  |
| ClaudeCode 코드검토지시문 작성                         | `02_AI_WORKFLOW_RULES.md` 7장               | `03_AI_OUTPUT_RULES.md` 3장, 5장, 7.5장, 10장 / `IMPLEMENTATION_GUARDRAILS.md` 관련 장       | 생략 없음                                                  |
| 지시문검토 요청                                      | `02_AI_WORKFLOW_RULES.md` 7장               | `03_AI_OUTPUT_RULES.md` 8.3장                                                          | 파일명 생성 없으면 Drive 저장 규칙 생략                              |
| 구현 완료보고 검토                                    | `02_AI_WORKFLOW_RULES.md` 7장, 8장           | `03_AI_OUTPUT_RULES.md` 7.6장, 10장 / `IMPLEMENTATION_GUARDRAILS.md` 15장, 16장           | 생략 없음                                                  |
| 사용자 테스트 안내 작성                                 | `02_AI_WORKFLOW_RULES.md` 7장               | `03_AI_OUTPUT_RULES.md` 7.7장 / Android 관련 시 `IMPLEMENTATION_GUARDRAILS.md` 7장, 12장    | 구현 관련 없으면 `IMPLEMENTATION` 일부 생략 가능                    |
| 보완루프 판단                                       | `02_AI_WORKFLOW_RULES.md` 8장               | 구현 관련 시 `IMPLEMENTATION_GUARDRAILS.md` 관련 장                                           | `03`은 새 문서 출력 없으면 생략                                   |
| 폐기 여부 판단                                      | `02_AI_WORKFLOW_RULES.md` 8장               | 폐기문서 작성 시 `03_AI_OUTPUT_RULES.md` 5.7장, 7.8장                                          | 구현 검토가 없으면 `IMPLEMENTATION` 생략                         |
| 폐기결정 문서 작성                                    | `02_AI_WORKFLOW_RULES.md` 8장               | `03_AI_OUTPUT_RULES.md` 3장, 5.7장, 7.8장                                                | `IMPLEMENTATION`은 폐기 대상이 구현가드레일과 관련 있을 때만 확인           |
| 인수인계 문서 작성                                    | `02_AI_WORKFLOW_RULES.md` 9장               | `03_AI_OUTPUT_RULES.md` 3장, 5장, 7.9장                                                  | 구현 가드레일은 구현 상태 요약 시 관련 장만 확인                           |
| MVP 구현로드맵 작성                                  | `02_AI_WORKFLOW_RULES.md` 6장, 11장          | `03_AI_OUTPUT_RULES.md` 5장, 7.10장, 9장                                                 | 구현 상세가 없으면 `IMPLEMENTATION` 생략                         |
| MVP 구현로드맵 완료반영판 작성                            | `02_AI_WORKFLOW_RULES.md` 7장, 11장          | `03_AI_OUTPUT_RULES.md` 5장, 7.11장, 9장                                                 | 구현 결과 요약 시 `IMPLEMENTATION` 관련 장 확인                    |
| 기준문서 개정                                       | `02_AI_WORKFLOW_RULES.md` 10장, 11장         | `03_AI_OUTPUT_RULES.md` 11장 / 구현가드레일 개정 시 `IMPLEMENTATION_GUARDRAILS.md` 전체           | 생략 없음                                                  |
| 파일명 생성 필요                                     | `03_AI_OUTPUT_RULES.md` 5장                 | 문서 종류별로 7장 또는 8장                                                                      | 판단 완료 후라면 `02` 일부 생략 가능                                |
| 빌드 파일명 생성 필요                                  | `03_AI_OUTPUT_RULES.md` 5.9장               | Android 빌드 수행 여부 또는 Android 실기기 테스트가 관련되면 `IMPLEMENTATION_GUARDRAILS.md` 12장 | 실제 Android 빌드 수행이 없고 파일명 생성만 필요한 경우 `IMPLEMENTATION` 생략 가능 |
| 문서블록·코드블록·채팅 직접출력 구분 필요                       | `03_AI_OUTPUT_RULES.md` 3장                 | 파일명 필요 시 5장                                                                           | 구현 관련 없으면 `IMPLEMENTATION` 생략                          |
| Drive 저장 위치 안내                                | `03_AI_OUTPUT_RULES.md` 6장                 | 파일명 필요 시 5장                                                                           | `02`, `IMPLEMENTATION`은 판단 완료 후 생략 가능                  |
| SaveData 변경 가능성 있음                            | `IMPLEMENTATION_GUARDRAILS.md` 9장          | `02_AI_WORKFLOW_RULES.md` 7장, 8장                                                      | 생략 없음                                                  |
| Scene 또는 ProjectSettings 변경 가능성 있음            | `IMPLEMENTATION_GUARDRAILS.md` 6장          | `02_AI_WORKFLOW_RULES.md` 7장                                                          | 생략 없음                                                  |
| UI, Canvas, Safe Area, Android A영역 관련         | `IMPLEMENTATION_GUARDRAILS.md` 7장, 8장, 12장 | `02_AI_WORKFLOW_RULES.md` 7장 / 출력 필요 시 `03` 7.4장 또는 7.7장                              | 생략 없음                                                  |
| 장비, 마법스크롤, MagicBook, Belt 관련                 | `IMPLEMENTATION_GUARDRAILS.md` 5장, 10장     | `02_AI_WORKFLOW_RULES.md` 7장, 8장                                                      | 생략 없음                                                  |
| 전투 로직 관련                                      | `IMPLEMENTATION_GUARDRAILS.md` 11장         | `02_AI_WORKFLOW_RULES.md` 7장, 8장                                                      | 생략 없음                                                  |
| Android native, APK, AAB, Gradle, Manifest 관련 | `IMPLEMENTATION_GUARDRAILS.md` 12장         | `02_AI_WORKFLOW_RULES.md` 7장 / 테스트 출력 시 `03` 7.7장                                     | 생략 없음                                                  |
| Git 명령어 안내                                    | `03_AI_OUTPUT_RULES.md` 3장 또는 4장           | `IMPLEMENTATION_GUARDRAILS.md` 13장                                                    | Codex·ClaudeCode 지시문이면 생략 없음                           |
| Git write 금지 확인                               | `IMPLEMENTATION_GUARDRAILS.md` 13장, 16장    | 완료보고 검토 시 `03_AI_OUTPUT_RULES.md` 10장                                                 | 생략 없음                                                  |
| 일반 질의응답, 짧은 판단, 문구 수정                         | `02_AI_WORKFLOW_RULES.md` 2장               | 필요 시 해당 장만 확인                                                                         | 파일명 생성 없으면 `03` 문서출력 규격 생략, 구현 없으면 `IMPLEMENTATION` 생략 |

---

## 5. 문서별 빠른 위치 색인

이 장은 기준문서별로 자주 확인하는 항목의 위치를 요약한다.

AI는 상황별 세부 라우팅표로 현재 요청 유형을 먼저 찾고, 필요 시 이 색인으로 세부 장을 확인한다.

### 5.1 `02_AI_WORKFLOW_RULES.md`

| 확인하려는 내용        | 볼 위치 |
| --------------- | ---- |
| 공통 판단 원칙        | 2장   |
| 기준문서 역할         | 3장   |
| AI별 역할          | 4장   |
| 어떤 AI를 쓸지 판단    | 5장   |
| 기획표준흐름          | 6장   |
| 작업표준흐름          | 7장   |
| 보완, 폐기, 방향점검 판단 | 8장   |
| 새 채팅에서 이어가기     | 9장   |
| 기준문서 개정 흐름      | 10장  |
| Claude 최종검토 기준  | 11장  |
| 예외 처리           | 12장  |
| 축약 운영           | 13장  |

### 5.2 `03_AI_OUTPUT_RULES.md`

| 확인하려는 내용            | 볼 위치     |
| ------------------- | -------- |
| 공통 출력 원칙            | 1장       |
| 일반 응답 형식            | 2장       |
| 문서블록, 코드블록, 채팅 직접출력 | 3장       |
| 터미널 명령어, Git 명령어    | 4장       |
| 소스 md 기준문서 파일명      | 4장 또는 5장 |
| Drive 저장문서 파일명      | 5장       |
| 빌드 파일명 기준           | 5.9장     |
| Drive 폴더 저장 기준      | 6장       |
| 검토세트                | 7.1장     |
| 검색세트                | 7.2장     |
| Claude 기안문 요청문      | 7.3장     |
| 구현지시문               | 7.4장     |
| 검토지시문               | 7.5장     |
| 완료보고 검토             | 7.6장     |
| 테스트 안내              | 7.7장     |
| 폐기결정 문서             | 7.8장     |
| 인수인계 문서             | 7.9장     |
| MVP 구현로드맵           | 7.10장    |
| MVP 구현로드맵 완료반영판     | 7.11장    |
| 세트문서 관리             | 8장       |
| 정규문서                | 9장       |
| 완료보고 요구사항           | 10장      |
| 기준문서 개정 출력          | 11장      |

### 5.3 `IMPLEMENTATION_GUARDRAILS.md`

| 확인하려는 내용                              | 볼 위치   |
| ------------------------------------- | ------ |
| 문서 목적과 적용 원칙                          | 1장, 3장 |
| 기준문서 4종 관계                            | 2장     |
| 프로젝트 정체성                              | 4장     |
| Legacy remnants                       | 5장     |
| Scene, Unity, ProjectSettings         | 6장     |
| UI, Canvas, Safe Area, Android A-Area | 7장     |
| UI Surface, Placeholder               | 8장     |
| SaveData                              | 9장     |
| Equipment, Magic, MagicBook, Belt     | 10장    |
| Combat                                | 11장    |
| Android native, APK, AAB              | 12장    |
| Git write 금지                          | 13장    |
| 구현 행동규칙                               | 14장    |
| 완료보고 필수 항목                            | 15장    |
| 최종 금지목록                               | 16장    |

---

## 6. AI별 기본 진입점

### 6.1 GPT

GPT는 Anima Spire 작업 흐름의 운영자다.

GPT는 먼저 이 문서에서 현재 요청의 상황별 라우팅 위치를 확인한다.

GPT는 `02_AI_WORKFLOW_RULES.md`에서 현재 요청의 작업 유형, 담당 AI, 사용자 결정 필요 여부를 판단한다.

응답, 문서, 지시문, 검토요청문, 검색요청문, 테스트 안내, Git 후속 안내를 출력해야 하는 경우 `03_AI_OUTPUT_RULES.md`를 따른다.

구현지시문 또는 코드검토지시문을 작성하는 경우 `IMPLEMENTATION_GUARDRAILS.md`를 반드시 반영한다.

### 6.2 Claude

Claude는 기획, 방향, 로드맵, 접근 방식 재검토, 정규문서 최종검토를 담당한다.

Claude는 이 문서에서 현재 요청의 상황별 라우팅 위치를 확인한다.

Claude는 `02_AI_WORKFLOW_RULES.md`에서 자신의 역할 범위와 사용자 명시 요청 예외 기준을 확인한다.

Claude가 기안문, 검토답변, 방향점검 답변을 작성해야 하는 경우 `03_AI_OUTPUT_RULES.md`의 문서 및 회신 규격을 따른다.

### 6.3 Gemini

Gemini는 기술적 타당성, 구조 리스크, 회귀 위험 검토에 사용한다.

Gemini에게 요청문을 작성하는 주체는 원칙적으로 GPT 또는 Claude다.

Gemini 요청문과 회신 규격은 `03_AI_OUTPUT_RULES.md`를 따른다.

Gemini 검토 대상이 구현 접근, 구조 리스크, 회귀 위험, SaveData, Scene, UI, Android, Combat, Equipment, Legacy code와 관련되는 경우에는 `IMPLEMENTATION_GUARDRAILS.md`의 관련 장을 함께 대조한다.

Gemini에게 전달하는 검토요청문에는 필요한 구현 가드레일 요약 또는 참조 장을 포함한다.

Gemini 검토 결과는 최종 결정이 아니며, GPT 또는 Claude가 해석한 뒤 사용자 결정에 따른다.

### 6.4 Perplexity

Perplexity는 외부 최신 정보, 공식 문서, 외부 이슈 사례, 정책·플랫폼 변화 검색에 사용한다.

Perplexity 검색요청문과 회신 규격은 `03_AI_OUTPUT_RULES.md`를 따른다.

Perplexity 검색 결과는 최종 결정이 아니며, GPT 또는 Claude가 출처와 프로젝트 기준을 함께 검토한 뒤 사용자 결정에 따른다.

### 6.5 Codex

Codex는 구현자다.

Codex는 GPT가 작성한 구현지시문 범위 안에서만 작업한다.

Codex는 `IMPLEMENTATION_GUARDRAILS.md`를 최우선 구현 가드레일로 따른다.

Codex는 Git write 작업을 수행하지 않는다.

Codex는 Android APK 또는 AAB 빌드를 수행하지 않는다.

### 6.6 ClaudeCode

ClaudeCode는 코드 구조 확인, 구현 결과 검토, 회귀 위험 검토를 담당한다.

파일 수정 없는 검토는 검토지시문 기준을 따른다.

파일 수정을 동반하는 작업은 구현지시문 기준을 따른다.

ClaudeCode는 `IMPLEMENTATION_GUARDRAILS.md`를 최우선 코드검토·구현 가드레일로 따른다.

ClaudeCode는 Git write 작업을 수행하지 않는다.

ClaudeCode는 Android APK 또는 AAB 빌드를 수행하지 않는다.

---

## 7. 출력 기준 진입 판단

다음 경우에는 `03_AI_OUTPUT_RULES.md`를 반드시 확인한다.

1. 파일명을 생성하는 Drive 저장문서를 작성하는 경우
2. 사용자가 저장하거나 다른 AI에게 전달할 문서 본문을 작성하는 경우
3. Claude, Gemini, Perplexity, Codex, ClaudeCode에 보낼 발신문서를 작성하는 경우
4. AI 회신 형식을 발신문서 안에 명시해야 하는 경우
5. 코드블록, 문서블록, 채팅 직접출력 방식을 구분해야 하는 경우
6. Drive 저장 위치를 안내해야 하는 경우
7. 정규문서, 인수인계, 폐기결정, MVP 구현로드맵, 완료반영판을 작성하는 경우
8. 터미널 명령어 또는 Git 후속 안내를 출력하는 경우

파일명을 생성하지 않는 일반 응답, 간단한 판단, 짧은 문구 수정, 단순 질의응답은 `03_AI_OUTPUT_RULES.md`의 문서 출력규격을 강제 적용하지 않는다.

단, 일반 응답이라도 사용자가 특정 출력형식, 문서블록, 코드블록, 파일명, 저장 위치를 명시적으로 요청한 경우에는 `03_AI_OUTPUT_RULES.md`의 해당 장을 확인한다.

---

## 8. 구현 가드레일 진입 판단

다음 경우에는 `IMPLEMENTATION_GUARDRAILS.md`를 반드시 확인한다.

1. Codex 구현지시문을 작성하는 경우
2. ClaudeCode 구현지시문을 작성하는 경우
3. ClaudeCode 코드검토지시문을 작성하는 경우
4. 완료보고에서 금지사항 위반 여부를 확인하는 경우
5. SaveData 변경 가능성이 있는 경우
6. Scene 또는 ProjectSettings 변경 가능성이 있는 경우
7. Android native 파일 또는 Android A영역 관련 작업이 있는 경우
8. UI_OverlayCanvas, SafeAreaUIRoot, Android Safe Area 관련 작업이 있는 경우
9. 전투 로직, 장비, 마법스크롤, 기존 legacy 코드에 영향을 줄 가능성이 있는 경우
10. Git write 금지, Android 빌드 금지, 완료보고 필수 항목을 확인해야 하는 경우
11. Gemini 기술검토가 구현 접근, 구조 리스크, 회귀 위험, SaveData, Scene, UI, Android, Combat, Equipment, Legacy code와 관련되는 경우

구현 또는 코드검토가 포함되는 작업에서는 `IMPLEMENTATION_GUARDRAILS.md`를 단순 참고가 아니라 필수 가드레일로 적용한다.

---

## 9. 문서 개정 작업 시 사용 기준

기준문서 자체를 작성하거나 개정하는 경우 다음 순서로 진행한다.

1. `02_AI_WORKFLOW_RULES.md`에서 기준문서 개정 흐름과 사용자 결정 필요 여부를 확인한다.
2. `03_AI_OUTPUT_RULES.md`에서 파일명, 문서블록, 코드블록, Drive 저장문서 기준을 확인한다.
3. 구현자 가드레일 내용이 포함되는 경우 `IMPLEMENTATION_GUARDRAILS.md`를 확인한다.
4. 사용자가 확정한 결정사항을 우선 반영한다.
5. Claude 최종검토가 필요한 문서인지 확인한다.
6. 신규 기준문서가 정합성 검토와 최종검토를 마치기 전까지 기존 기준문서를 제거하지 않는다.

기준문서 최종본에는 개편 사유를 장황하게 쓰지 않는다.

개편 경위는 변경 이력에 간단히 기록한다.

---

## 10. 충돌 및 확인 불가 상황

AI는 다음 상황에서 임의로 진행하지 않는다.

1. 기준문서 간 내용이 충돌하는 경우
2. 사용자 결정사항과 문서 기준이 충돌하는 경우
3. Drive 문서 내용과 GitHub 소스 기준문서가 충돌하는 경우
4. 코드 상태를 직접 확인하지 못한 경우
5. Git 상태를 직접 확인하지 못한 경우
6. Unity 또는 Android 테스트 결과를 확인하지 못한 경우
7. 작업 범위 밖 변경이 필요해 보이는 경우
8. 구현 가드레일상 금지된 작업이 필요해 보이는 경우

이 경우 AI는 충돌 또는 확인 불가 항목을 분리하여 사용자에게 보고하고, 사용자 결정을 요청한다.

---

## 11. 변경 이력

v01

* Anima Spire 기준문서 4종 체계 도입을 위한 임시 라우터 초안 작성.
* 판단기준과 출력기준을 직접 포함하지 않고, 문서별 참조 방향만 정의.

v02

* `02_AI_WORKFLOW_RULES.md`, `03_AI_OUTPUT_RULES.md`, `IMPLEMENTATION_GUARDRAILS.md` 논의 결과를 반영하여 라우터 재정리.
* 문서 단위 라우팅을 장 번호 단위 라우팅으로 보강.
* 파일명을 생성하는 Drive 저장문서의 출력기준 진입 조건 추가.
* 구현 가드레일 진입 조건 정리.
* Perplexity 역할과 AI 회신 규격 참조 기준 반영.
* Gemini 기술검토가 구현 구조, 회귀 위험, SaveData, Scene, UI, Android, Combat, Equipment, Legacy code와 관련되는 경우 `IMPLEMENTATION_GUARDRAILS.md`를 함께 대조하도록 라우팅 기준 보완.
