import pygame 
import os
###################################################################################
#기본 초기화 (반드시 해야 하는 것들)
pygame.init() 

#화면 크기 설정
screen_width=480 # 가로 크기 
screen_height=640 # 세로 크기 
screen=pygame.display.set_mode((screen_width,screen_height)) 


#화면 타이틀 설정
pygame.display.set_caption("게임 이름") 

#FPS
clock = pygame.time.Clock()
###################################################################################

# 1. 사용자 게임 초기화 (배경 화면, 게임 이미지, 좌표, 속도, 폰트 등) 
current_path = os.path.dirname(__file__) # 현재 파일의 위치 반환 
image_path = os.path.join(current_path, "이미지") # 이미지 폴더 위치 반환 

# ball = pygame.image.load(os.path.join(image_path, "공png.png")) 
# ball_size = ball.get_rect().size
# ball_width = ball_size[0]
# ball_height = ball_size[1]
# ball_pos_x = (screen_width / 2) - (ball_width /2)
# ball_pos_y = screen_height - ball_height


ball_radius = 30
ball_pos = [(screen_width/2),screen_height - ball_radius]
ball_to_x = 3
ball_to_y = -6
ball_width = ball_radius *2
ball_height = ball_radius *2
# ball = get_rect(screen, (0,255,255), ball_pos, ball_radius)

ball2_radius = 20
ball2_pos = [(screen_width/3),screen_height - ball2_radius]
ball2_to_x = -3
ball2_to_y = -6
ball2_width = ball2_radius *2
ball2_height = ball2_radius *2
ball2 = pygame.draw.circle(screen, (0,255,255), ball2_pos, ball2_radius)


running=True 
while running:  
    dt = clock.tick(30) 

    # 2. 이벤트 처리 (키보드, 마우스 등)                     
    for event in pygame.event.get():
        if event.type == pygame.QUIT:  
            running = False 
        
    # 3. 게임 캐릭터 위치 정의
    # ball_rect = ball.get_rect()
    # ball.left = s]s]
    if ball_pos[0] - ball_radius <= 0 or ball_pos[0] - ball_radius > screen_width - ball_width: 
        ball_to_x = ball_to_x * -1 

    #땅바닥 닿을경우
    if ball_pos[1] >= screen_height - ball_radius: 
        ball_to_y = -18 
    else: # 그 외의 모든 경우에는 속도를 증가
        ball_to_y += 0.5 

    ball_pos[0] += ball_to_x
    ball_pos[1] += ball_to_y

    if ball2_pos[0] - ball2_radius <= 0 or ball2_pos[0] - ball2_radius > screen_width - ball2_width: 
        ball2_to_x = ball2_to_x * -1 

    #땅바닥 닿을경우
    if ball2_pos[1] >= screen_height - ball2_radius: 
        ball2_to_y = -18 
    else: # 그 외의 모든 경우에는 속도를 증가
        ball2_to_y += 0.5 

    ball2_pos[0] += ball2_to_x
    ball2_pos[1] += ball2_to_y


    # 4. 충돌 처리
    # 공 rect 정보
    # ball_rect = ball.get_rect() 
    # ball_rect.left = ball_pos[0]
    # ball_rect.top = ball_pos[1]
    # # 공2 rect 정보
    # ball2_rect = ball2.get_rect() 
    # ball2_rect.left = ball2_pos[0]
    # ball2_rect.top = ball2_pos[1]

    # 공과 캐릭터 충돌 처리
    if ball_rect.colliderect(ball2_rect): 
        running = False
        break

    # 5. 화면에 그리기
    screen.fill((0,0,0))
    # screen.blit(ball, (ball_pos_x, ball_pos_y))
    pygame.draw.circle(screen, (0,255,255), ball_pos, ball_radius)
    pygame.draw.circle(screen, (0,0,255), ball2_pos, ball2_radius)
    
    pygame.display.update() 


pygame.quit()

