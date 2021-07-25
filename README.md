# COM3D2.PresetLoadCtr.Plugin

BepInEx plugin  

2.3.0 전용  

https://youtu.be/v10GHR0BDmo

![2021-06-08 20 23 53](https://user-images.githubusercontent.com/20321215/121176722-8318f580-c897-11eb-887a-5a6d9834b71e.png)


원래 프리셋 저장시 옷만 / 몸만 / 둘다 세종류만 저장 가능  

근데 실제 프리셋 데이터엔 몸/옷 정보 둘다 들어가 있음  

프리셋 읽어올때 옷만 일어오게끔 설정된 프리셋을 강제로 옷만 / 몸만 / 둘다 로 바궈서 읽어오게끔 바꿔주는거  

덤으로 신규 고용시(스카우트 새로 생성 포함) 현제 가진 프리셋중 랜덤으로 자동 적용됨  (안되는거 같다)

- Random Preset Run Auto : 신규 생성시 자동 적용(안되는거 같다)
- Random Preset Run : 목록에서 랜덤 프리셋 적용
- List load : 리스트 재로딩
- preset save : 프리셋 저장
- PresetType : 프리셋 읽어올때 어떻게 일어올지? 프리셋 파일 기본값 / 옷만  몸만 / 옷 몸 둘다
- ListType : "Random Preset Run" 사용시 참조할 목록
- ModType : 적용할 메이드. 선택한 메이드 / 모든 메이드에게 똑같은옷 / 모든 메이드에게 다른옷


# 설치 위치

COM3D2\BepInEx\plugins


# 필요한거

- BepInEx https://github.com/BepInEx/BepInEx  
- SybarisLoader https://github.com/BepInEx/BepInEx.SybarisLoader.Patcher  
- UnityInjectorLoader https://github.com/BepInEx/BepInEx.UnityInjectorLoader  

- COM3D2.API.dll  https://github.com/DeathWeasel1337/COM3D2_Plugins/releases/download/v3/COM3D2.API.v1.0.zip
- LillyUtill 21.7.22 over https://github.com/customordermaid3d2/COM3D2.LillyUtill/releases  
