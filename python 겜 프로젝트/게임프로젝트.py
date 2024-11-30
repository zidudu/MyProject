# 5. 모든 공을 없애면 게임 종료 (성공)
# 6. 캐릭터는 공에 닿으면 게임 종료 (실패)
# 7. 시간 제한 99초 초과 시 게임 종료 (실패)


import os
import pygame 
###################################################################################
#기본 초기화 (반드시 해야 하는 것들)
pygame.init() 

#화면 크기 설정
screen_width=640 # 가로 크기 
screen_height=480 # 세로 크기 
screen=pygame.display.set_mode((screen_width,screen_height)) 


#화면 타이틀 설정
pygame.display.set_caption("pang 게임") 

#FPS
clock = pygame.time.Clock()
###################################################################################

# 1. 사용자 게임 초기화 (배경 화면, 게임 이미지, 좌표, 속도, 폰트 등) 
current_path = os.path.dirname(__file__) # 현재 파일의 위치 반환 
image_path = os.path.join(current_path, "이미지") # 이미지 폴더 위치 반환 

# 배경 만들기
background = pygame.image.load(os.path.join(image_path, "게임배경.jpg")) 

# 스테이지 만들기
stage = pygame.image.load(os.path.join(image_path, "무대.png"))
stage_size = stage.get_rect().size # 사이즈 정보
stage_height = stage_size[1] # 스테이지의 높이 위에 캐릭터를 두기 위해 사용 

#캐릭터 만들기
character = pygame.image.load(os.path.join(image_path, "캐릭터.png"))
character_size = character.get_rect().size
character_width = character_size[0]
character_height = character_size[1]
character_x_pos = (screen_width/2) - (character_width/2)
character_y_pos = screen_height - character_height - stage_height

#캐릭터 이동 방향
character_to_x = 0 #y는 없어도 됨. 위아래로 움직이지 않음.근데 점프 넣을거면 y만드는게 좋음

#캐릭터 이동 속도
character_speed = 5

# 무기 만들기
weapon = pygame.image.load(os.path.join(image_path, "무기.png"))
weapon_size = weapon.get_rect().size
weapon_width = weapon_size[0]

# 무기는 한 번에 여러 발 발사 가능
weapons = []

# 무기 이동 속도
weapon_speed = 10

# 공 만들기 (4개 크기에 대해서 따로 처리)
ball_images = [ 
    pygame.image.load(os.path.join(image_path, "공1.png")),
    pygame.image.load(os.path.join(image_path, "공2.png")),
    pygame.image.load(os.path.join(image_path, "공3.png")),
    pygame.image.load(os.path.join(image_path, "공4.png"))
]
#처음 스피드가 높음

# 공 크기에 따른 최초 스피드
ball_speed_y = [-18, -15, -12, -9] # index 0, 1, 2, 3 에 해당하는 값
#공이 바닥에 튕겼을때 바닥에 튕기고 나서 올라갈 때는 y값이 마이너스로 빠져야 하니 -로 함

#공 크기에 따른 최초 스피드
balls = []

# 최초 발생하는 큰 공 추가
balls.append({
    "pos_x" : 50, #공의 x 좌표
    "pos_y" : 50, #공의 y 좌표 #공은 왼쪽 위에서 시작됨 
    "img_idx" : 0, 
    #나중에 공이 나눠지면 이미지 인덱스 값도 늘어날 것이다
    "to_x" : 3, # x축 이동방향, -3 이면 왼쪽으로, 3이면 오른쪽으로 # 공은 처음에 시작할때 오른쪽으로 이동하게 됨 
    "to_y" : -6, # y축 이동방향, #위로 올라가면 -방향, 아래로 내려가면 +방향 #y좌표는 처음에는 위로 올라가게 됨
    "init_spd_y" : ball_speed_y[0] })# y 최초 속도 #속도는 공마다 다름 # -18


# 사라질 무기, 공 정보 저장 변수
weapon_to_remove = -1 
ball_to_remove = -1

# 폰트 정의
font = pygame.font.Font(None, 50)
#게임 종료 메세지 
# TimeOut(시간 초과 성공)
# Mission Complete(성공)
# Game Over (캐릭터 공에 맞음, 실패)
game_result = "Game Over"
#총 시간
total_time = 100

#시작 시간
start_ticks = pygame.time.get_ticks() #시작 시간 정의
#캐릭터 움직임
moving = None

# 즉시 종료
now_stop = False

# 부스트 대시
dash = 5

running=True 
while running:  
    dt = clock.tick(30) 

    # 2. 이벤트 처리 (키보드, 마우스 등)                     
    for event in pygame.event.get():
        if event.type == pygame.QUIT:  
            now_stop = True
            running = False
            
        if event.type == pygame.KEYDOWN:
            # 왼쪽 이동
            if event.key == pygame.K_LEFT: #캐릭터를 왼쪽으로
                character_to_x -= character_speed
                character = pygame.image.load(os.path.join(image_path, "캐릭터(-걷기오른발).png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
            # 왼쪽 대시    
            elif event.key == pygame.K_z:
                character_to_x -= (character_speed + dash) 
            # 오른쪽 이동
            elif event.key == pygame.K_RIGHT: #캐릭터를 오른쪽으로
                character_to_x += character_speed
                character = pygame.image.load(os.path.join(image_path, "캐릭터(+걷기오른발).png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
            # 오른쪽 대시    
            elif event.key == pygame.K_x:
                character_to_x += (character_speed + dash) 
            # 앉는 모션
            elif event.key == pygame.K_DOWN: # 버그가 생김. 왼쪽,오른쪽 방향키를 누르고 있다가 아래를 누르면 쭈구려진 상태에서 이동됨
                character = pygame.image.load(os.path.join(image_path, "캐릭터(쭈구리기).png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
                character_to_x = 0
            # 무기 발사(스페이스)
            elif event.key == pygame.K_SPACE: #스페이스를 눌렀을때 이 변수들이 적용됨 
                character_to_x = 0
                character = pygame.image.load(os.path.join(image_path, "캐릭터(무기쏘기).png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
            
                weapon_x_pos = character_x_pos + (character_width/2) - (weapon_width/2)
                weapon_y_pos = character_y_pos 
                weapons.append([weapon_x_pos, weapon_y_pos]) #무기 여러개 쓰면 좌표 여러개 넣게 됨
        # 방향키 손 땔때        
        if event.type == pygame.KEYUP:
            # 왼쪽 오른쪽 방향키 손 땔때
            if event.key == pygame.K_LEFT or event.key == pygame.K_RIGHT:
                character_to_x = 0
                character = pygame.image.load(os.path.join(image_path, "캐릭터.png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
            # 앉는 키 땔때    
            elif event.key == pygame.K_DOWN: #  버그 고침
                character_to_x = 0
                character = pygame.image.load(os.path.join(image_path, "캐릭터.png"))
                character_size = character.get_rect().size
                character_height = character_size[1]
                character_y_pos = screen_height - stage_height - character_height
            # 대시 버튼 손 땠을때
            elif event.key == pygame.K_z or event.key == pygame.K_x:
                character_to_x = 0

    # 3. 게임 캐릭터 위치 정의
    character_x_pos += character_to_x
    #가로 경계선
    if character_x_pos < 0:
        character_x_pos = 0
    elif character_x_pos > screen_width - character_width:
        character_x_pos = screen_width - character_width

    #무기 위치 조정
    weapons = [[w[0], w[1] - weapon_speed] for w in weapons] 
               
    # 천장에 닿은 무기 없애기 (무기 위치 조정 밑에 있어야 됨)
    weapons = [ [w[0], w[1]] for w in weapons if w[1] > 0] 
    # weapons = [ [w[0], w[1]] for w in weapons if w[1] > -430] #이렇게 쓰면 무기가 끝까지 하늘로 올라가게 됨. 필요시 사용 

    # 공 위치 정의
    for ball_idx, ball_val in enumerate(balls): 
        ball_pos_x = ball_val["pos_x"] 
        ball_pos_y = ball_val["pos_y"] 
        ball_img_idx = ball_val["img_idx"]
        #이렇게 되면 공의 x좌표,y좌표 그리고 현재 공의 이미지 위치 정보를 알 수 있게 됨
        ball_size = ball_images[ball_img_idx].get_rect().size # 공1의 사이즈를 가져옴
        ball_width = ball_size[0]
        ball_height = ball_size[1]

        #가로벽에 닿았을 때 공 이동 위치 변경 (튕겨 나오는 효과)
        if ball_pos_x <= 0 or ball_pos_x > screen_width - ball_width: 
            ball_val["to_x"] = ball_val["to_x"] * -1 

        # 세로 위치 (무대에 닿으면 튕겨져 나감)
        # 스테이지에 튕겨서 올라가는 처리
        if ball_pos_y >= screen_height - stage_height - ball_height: 
            ball_val["to_y"] = ball_val["init_spd_y"] 
        else: # 그 외의 모든 경우에는 속도를 증가
            ball_val["to_y"] += 0.5 
        #공 위치
        ball_val["pos_x"] += ball_val["to_x"]
        ball_val["pos_y"] += ball_val["to_y"]
   
    # 4. 충돌 처리

    # 캐릭터 rect 정보 업데이트
    character_rect = character.get_rect()
    character_rect.left = character_x_pos
    character_rect.top = character_y_pos

    # 공들의 좌표 설정 (원래는 공의 x,y좌표가 있지만 충돌처리에서 따로 설정해서 씀)
    for ball_idx, ball_val in enumerate(balls): 
        ball_pos_x = ball_val["pos_x"] 
        ball_pos_y = ball_val["pos_y"] 
        ball_img_idx = ball_val["img_idx"]

        # 공 정보 업데이트
        ball_rect = ball_images[ball_img_idx].get_rect() 
        ball_rect.left = ball_pos_x
        ball_rect.top = ball_pos_y

        # 공과 캐릭터 충돌 처리
        if character_rect.colliderect(ball_rect): 
            running = False
            break
        
        # 공과 무기들 충돌 처리 (무기는 여러발을 쏘기 때문에 무기 리스트에 있는 값을 가져와서 해야 함)
        for weapon_idx, weapon_val in enumerate(weapons): #weapon_idx는 0, weapon_val은 0번째 리스트 내용
            weapon_pos_x = weapon_val[0] #x좌표 가져옴 
            weapon_pos_y = weapon_val[1] # y좌표 가져옴

            # 무기 rect 정보 업데이트
            weapon_rect = weapon.get_rect()
            weapon_rect.left = weapon_pos_x
            weapon_rect.top = weapon_pos_y

            #충돌 체크
            if weapon_rect.colliderect(ball_rect): #이 파트에서는 무기가 공에 닿으면 공이 없어지는 프로그램을 만들거임
                weapon_to_remove = weapon_idx # 해당 무기 없애기 위한 값 설정 
                ball_to_remove = ball_idx # 해당 공 없애기 위한 값 설정

                # 가장 작은 크기의 공이 아니라면 다음 단계의 공으로 나눠주기
                if ball_img_idx < 3:
                    # 현재 공 크기 정보를 가지고 옴
                    ball_width = ball_rect.size[0]
                    ball_height = ball_rect.size[1]

                    # 나눠진 공 정보
                    small_ball_rect = ball_images[ball_img_idx + 1].get_rect() 
                    small_ball_width = small_ball_rect.size[0] # 
                    small_ball_height = small_ball_rect.size[1]

                    # 왼쪽으로 튕겨나가는 작은 공 
                    balls.append({
                        "pos_x" : ball_pos_x + (ball_width / 2) - (small_ball_width),
                        "pos_y" : ball_pos_y + (ball_height / 2) - (small_ball_height / 2),  
                        "img_idx" : ball_img_idx + 1, # 공의 이미지 인덱스 # 공 2번째꺼
                        "to_x" : -3, #x축 이동방향 # 이 공은 왼쪽으로 튕기니까 -3으로 해줌  
                        "to_y" : -6, #y축 이동방향
                        "init_spd_y" : ball_speed_y[ball_img_idx + 1] })

                    # 오른쪽으로 튕겨나가는 작은 공
                    balls.append({
                        "pos_x" : ball_pos_x + (ball_width / 2) - (small_ball_width), #공의 x 좌표
                        "pos_y" : ball_pos_y + (ball_height / 2) - (small_ball_height / 2), #공의 y 좌표 
                        "img_idx" : ball_img_idx + 1, 
                        "to_x" : 3, # x축 이동방향, 
                        "to_y" : -6, # y축 이동방향
                        "init_spd_y" : ball_speed_y[ball_img_idx + 1] })

                break # 브레이크를  함으로써 밑으로 내려갈줄 알았는데 무기정보for문이 브레이크되서 위쪽에 있는 공 좌표 설정for문이 다음 공 정보로 또 넘어와서 계속 진행하게 됨
        else: #계속 게임을 진행
            continue #안쪽 for 문 조건이 맞지 않으면 continue, 바깥 for 문 계속 수행
        break # 안쪽 for 문에서 break를 만나면 여기로 진입 가능. 2중 for 문을 한번에 탈출
    
    # 충돌된 공 or 무기 없애기
    if ball_to_remove > -1: 
        del balls[ball_to_remove] 
        ball_to_remove = -1 
    
    if weapon_to_remove > -1:
        del weapons[weapon_to_remove] 
        weapon_to_remove = -1 # 공을 없애고 다시 원위치

    # 모든 공을 없앤 경우 게임 종료 (성공)
    if len(balls) == 0: # balls 리스트에 사전이 더이상 없는 경우
        game_result = "Mission Complete"
        running = False





    # 5. 화면에 그리기
    #배경 그려내기
    screen.blit(background, (0,0))
    #무기 그려내기
    for weapon_x_pos, weapon_y_pos in weapons:  # weapons에서 x좌표와 y좌표를 가져옴.
        screen.blit(weapon, (weapon_x_pos, weapon_y_pos))
    # 공 그려내기
    for idx, val in enumerate(balls):
        ball_pos_x = val["pos_x"]
        ball_pos_y = val["pos_y"]
        ball_img_idx = val["img_idx"]
        screen.blit(ball_images[ball_img_idx], (ball_pos_x,ball_pos_y))
    #무대 그려내기
    screen.blit(stage, (0, screen_height - stage_height)) 
    #캐릭터 그려내기
    screen.blit(character, (character_x_pos, character_y_pos))

    # 경과된 시간
    elapsed_time = (pygame.time.get_ticks() - start_ticks) /1000 # 초 단위가 됨
    # 시간 렌더링
    timer = font.render("Time : {} ".format(int(total_time - elapsed_time)), True, (255,255,255))
    # 시간 그려내기
    screen.blit(timer, (10,10))
    
    # 시간 초과했다면
    if total_time - elapsed_time <= 0:
        game_result = "Time Over" # 원래는 game over였는데 바꿈
        running = False
        break
    pygame.display.update()

# x 버튼 안누르는 경우
if now_stop == False:
    # 게임 오버 메세지 저장소
    msg  = font.render(game_result, True, (255,255,0) ) # 노란색
    msg_rect = msg.get_rect(center=(int(screen_width / 2), int(screen_height /2))) # 글자가 정중앙에 오게 함
    screen.blit(msg, msg_rect) # msg_rect안에 좌표가 있음
    pygame.display.update() # 업데이트를 꼭 해줘야함

    # 2초 대기
    pygame.time.delay(2000)
#프로그램 종료
pygame.quit()

