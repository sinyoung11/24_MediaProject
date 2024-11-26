# AIRogueRPG

아주대학교 미디어프로젝트 AI삼총사 팀의 **AI를 활용한 사용자 맞춤형 레벨디자인 시스템** 프로젝트 Unity Client 게임입니다.   
<br>
![AIRogueRPG Start screen](https://github.com/user-attachments/assets/8f18a2da-12aa-4592-9c34-2deaed681206)   
최종 버전은 **chan** 브랜치에 있습니다.   
<br>

## Demonstration video
아래 링크에서 게임 플레이 시연 영상을 보실 수 있습니다.   
[Game play video](https://drive.google.com/file/d/1dbkrpeSnY2HxEVRgSfTixXE1AOX4Kg8K/view?usp=sharing/)

<br>

## Server repository   
- 유니티 플레이 데이터를 Flask server가 받아서, Langchain model의 input으로 전달합니다.   
- Langchain model은 플레이 데이터를 통해 유저의 플레이 수준을 분석하고 새로운 레벨디자인을 제시합니다.   
- Langchain model의 응답은 Flask server를 통해 유니티에 전달되어 반영됩니다.
<br>
[Unity_LangChain Repository](https://github.com/KimJinWooDa/Unity_LangChain)        

<br>      
<br>  

## Run the program
해당 프로젝트를 정상적으로 실행시키기 위해서는   
1. 랭체인 서버 레포지토리에 있는 **flask_s.py** 를 실행시키고 ("python flask_s.py" command 입력)   
2. 랭체인 서버 레포지토리에 있는 **langchain_s.py** 를 실행시킨 뒤 ("python langchain_s.py " command 입력)   
3. 유니티 클라이언트 게임 레포지토리의 유니티 프로그램을 실행하셔야 합니다.

### OpenAI API key 
langchain_s.py 의 open_ai_key 사용에 문제가 발생하거나, 자신의 OpenAI api key를 사용하고 싶으시다면     
[OpenAI API key](https://platform.openai.com/api-keys)   
위 사이트에서 발급받은 api key를 langchain_s.py 의 26번 line, 'openai_api_key' 부분에 입력해주세요.
