import os
import pygame

# 전역 변수 설정
screen_width = 640  # 화면의 가로 크기
screen_height = 480  # 화면의 세로 크기
screen = None  # 화면 객체
clock = None  # FPS 설정을 위한 Clock 객체
current_path = None  # 현재 파일의 디렉토리 경로
image_path = None  # 이미지 폴더의 경로
character_images = {}  # 캐릭터 이미지들을 저장할 딕셔너리
ball_images = []  # 공 이미지들을 저장할 리스트
weapon = None  # 무기 이미지
stage = None  # 스테이지 이미지
font = None  # 폰트 객체

# 캐릭터 이동 관련 변수
character_to_x = 0  # 캐릭터의 이동 방향 변수
left_pressed = False  # 왼쪽 키가 눌렸는지 여부
right_pressed = False  # 오른쪽 키가 눌렸는지 여부
character = None  # 현재 캐릭터 이미지

def init_game():
    """게임 초기화 및 설정"""
    global screen, clock, current_path, image_path, character_images, ball_images, weapon, stage, font

    pygame.init()  # Pygame 라이브러리 초기화
    screen = pygame.display.set_mode((screen_width, screen_height))  # 화면 설정
    pygame.display.set_caption("Pang 게임")  # 게임 제목 설정
    clock = pygame.time.Clock()  # FPS 설정을 위한 Clock 객체 생성

    current_path = os.path.dirname(__file__)  # 현재 파일의 위치 반환
    image_path = os.path.join(current_path, "이미지")  # 이미지 폴더 위치 반환

    # 게임에 필요한 이미지 로드
    load_images()

    # 폰트 객체 생성
    font = pygame.font.Font(None, 50)

def load_images():
    """게임에 필요한 이미지 로드"""
    global character_images, ball_images, weapon, stage

    # 캐릭터 이미지 로드 및 딕셔너리에 저장
    character_images["default"] = pygame.image.load(os.path.join(image_path, "캐릭터.png"))
    character_images["left"] = pygame.image.load(os.path.join(image_path, "캐릭터(-걷기오른발).png"))
    character_images["right"] = pygame.image.load(os.path.join(image_path, "캐릭터(+걷기오른발).png"))
    character_images["down"] = pygame.image.load(os.path.join(image_path, "캐릭터(쭈구리기).png"))
    character_images["attack"] = pygame.image.load(os.path.join(image_path, "캐릭터(무기쏘기).png"))

    # 공 이미지 로드 및 리스트에 추가
    ball_images.append(pygame.image.load(os.path.join(image_path, "축구공.png")))  # 가장 큰 공
    ball_images.append(pygame.image.load(os.path.join(image_path, "공2.png")))  # 두 번째 크기 공
    ball_images.append(pygame.image.load(os.path.join(image_path, "공3.png")))  # 세 번째 크기 공
    ball_images.append(pygame.image.load(os.path.join(image_path, "공4.png")))  # 가장 작은 공

    # 무기와 스테이지 이미지 로드
    weapon = pygame.image.load(os.path.join(image_path, "무기.png"))
    stage = pygame.image.load(os.path.join(image_path, "무대.png"))

def main():
    """메인 게임 루프"""
    global character_to_x, character  # 전역 변수 선언
    init_game()  # 게임 초기화 함수 호출

    # 캐릭터에 대한 초기 설정
    character = character_images["default"]  # 기본 캐릭터 이미지 설정
    character_width = character.get_rect().size[0]  # 캐릭터의 가로 크기
    character_x_pos = (screen_width / 2) - (character_width / 2)  # 화면 중앙에 캐릭터 위치 설정
    character_speed = 5  # 캐릭터의 이동 속도
    dash_speed = 5  # 대시 시 추가 속도

    # 무기에 대한 초기 설정
    weapon_width = weapon.get_rect().size[0]  # 무기의 가로 크기
    weapons = []  # 발사된 무기를 담을 리스트
    weapon_speed = 10  # 무기의 이동 속도

    # 공에 대한 초기 설정
    ball_speed_y = [-18, -15, -12, -9]  # 공의 크기별 y축 초기 속도
    balls = []  # 화면에 존재하는 공들을 담을 리스트

    # 최초로 발생하는 큰 공 추가
    balls.append({
        "pos_x": 50,  # 공의 x 좌표
        "pos_y": 50,  # 공의 y 좌표
        "img_idx": 0,  # 사용될 공 이미지 인덱스 (0: 가장 큰 공)
        "to_x": 3,  # 공의 x축 이동 방향 (양수: 오른쪽, 음수: 왼쪽)
        "to_y": -6,  # 공의 y축 이동 방향 (초기에는 살짝 위로 올라갔다 내려옴)
        "init_spd_y": ball_speed_y[0],  # y축 초기 속도 (공마다 다름)
    })

    weapon_to_remove = -1  # 제거될 무기의 인덱스
    ball_to_remove = -1  # 제거될 공의 인덱스

    total_time = 100  # 제한 시간 (초)
    start_ticks = pygame.time.get_ticks()  # 게임 시작 시간
    game_result = "Game Over"  # 기본 게임 종료 메시지
    now_stop = False  # 게임 즉시 종료 여부

    running = True  # 게임 실행 여부
    while running:
        dt = clock.tick(30)  # 게임의 FPS 설정 (초당 30프레임)

        # 이벤트 처리
        running, now_stop, weapons = handle_events(
            character_speed, dash_speed, character_images, weapons, character_x_pos, character_width, weapon_width
        )

        # 캐릭터 위치 업데이트
        character_x_pos, character_y_pos = update_character_position(character_x_pos, character_to_x, character_width)

        # 무기 위치 업데이트
        weapons = update_weapons(weapons, weapon_speed)

        # 공 위치 업데이트
        balls = update_balls(balls, ball_speed_y)

        # 충돌 처리
        weapon_to_remove, ball_to_remove, game_result, running = check_collisions(
            character_x_pos, character_y_pos, character, balls, weapons, weapon_to_remove, ball_to_remove, game_result, running, ball_speed_y
        )

        # 모든 공을 제거한 경우 게임 종료
        if len(balls) == 0:
            game_result = "Mission Complete"
            running = False

        # 화면에 그리기
        draw_screen(character_x_pos, character_y_pos, character, weapons, balls)

        # 경과 시간 계산
        elapsed_time = (pygame.time.get_ticks() - start_ticks) / 1000  # 초 단위로 변환

        # 제한 시간 초과 시 게임 종료
        if total_time - elapsed_time <= 0:
            game_result = "Time Over"
            running = False

        # 타이머 그리기
        draw_timer(total_time, elapsed_time)

        # 화면 업데이트
        pygame.display.update()

    # 게임 종료 메시지 표시
    show_game_over(game_result, now_stop)

def handle_events(character_speed, dash_speed, character_images, weapons, character_x_pos, character_width, weapon_width):
    """이벤트 처리"""
    global character_to_x, left_pressed, right_pressed, character

    now_stop = False
    running = True

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            now_stop = True  # 즉시 종료 여부 설정
            running = False  # 게임 루프 종료

        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_LEFT:  # 왼쪽 방향키
                character_to_x = -character_speed
                left_pressed = True
                character = character_images["left"]  # 왼쪽 이동 이미지로 변경
            if event.key == pygame.K_RIGHT:  # 오른쪽 방향키
                character_to_x = character_speed
                right_pressed = True
                character = character_images["right"]  # 오른쪽 이동 이미지로 변경
            if event.key == pygame.K_z:  # 왼쪽 대시
                character_to_x = -character_speed - dash_speed
                character = character_images["left"]
            if event.key == pygame.K_x:  # 오른쪽 대시
                character_to_x = character_speed + dash_speed
                character = character_images["right"]
            if event.key == pygame.K_DOWN:  # 앉기
                character = character_images["down"]
                character_to_x = 0  # 이동 멈춤
            if event.key == pygame.K_SPACE:  # 무기 발사
                character = character_images["attack"]

                weapon_x_pos = character_x_pos + (character_width / 2) - (weapon_width / 2)
                # weapon_y_pos는 캐릭터의 y 위치에서 계산해야 하므로, update_character_position() 후에 설정
                weapons.append([weapon_x_pos, 0])  # 임시로 y 위치를 0으로 설정

        if event.type == pygame.KEYUP:
            if event.key == pygame.K_LEFT and left_pressed:
                character_to_x = 0
                left_pressed = False
                character = character_images["default"]  # 기본 이미지로 변경
            if event.key == pygame.K_RIGHT and right_pressed:
                character_to_x = 0
                right_pressed = False
                character = character_images["default"]
            if event.key == pygame.K_DOWN:
                character = character_images["default"]
            if event.key == pygame.K_z or event.key == pygame.K_x:
                character_to_x = 0
                character = character_images["default"]

    return running, now_stop, weapons

def update_character_position(character_x_pos, character_to_x, character_width):
    """캐릭터 위치 업데이트"""
    character_x_pos += character_to_x  # 이동 방향에 따른 위치 변경

    # 화면 경계 내에 위치하도록 설정
    if character_x_pos < 0:
        character_x_pos = 0
    elif character_x_pos > screen_width - character_width:
        character_x_pos = screen_width - character_width

    # 캐릭터의 높이와 y 위치 업데이트
    character_height = character.get_rect().size[1]
    character_y_pos = screen_height - character_height - stage.get_rect().size[1]

    return character_x_pos, character_y_pos

def update_weapons(weapons, weapon_speed):
    """무기 위치 업데이트"""
    # 무기를 위로 이동
    weapons = [[w[0], w[1] - weapon_speed] for w in weapons]

    # 화면 밖으로 벗어난 무기 제거
    weapons = [ [w[0], w[1]] for w in weapons if w[1] > 0]
    return weapons

def update_balls(balls, ball_speed_y):
    """공 위치 업데이트"""
    for ball_val in balls:
        ball_pos_x = ball_val["pos_x"]  # 공의 x 좌표
        ball_pos_y = ball_val["pos_y"]  # 공의 y 좌표
        ball_img_idx = ball_val["img_idx"]  # 공의 이미지 인덱스

        ball_size = ball_images[ball_img_idx].get_rect().size  # 공의 크기
        ball_width = ball_size[0]
        ball_height = ball_size[1]

        # 가로벽에 부딪혔을 때 방향 변경
        if ball_pos_x <= 0 or ball_pos_x >= screen_width - ball_width:
            ball_val["to_x"] *= -1  # 이동 방향 반전

        # 세로 위치 (바닥에 닿았을 때)
        if ball_pos_y >= screen_height - stage.get_rect().size[1] - ball_height:
            ball_val["to_y"] = ball_val["init_spd_y"]  # 초기 속도로 설정
        else:
            ball_val["to_y"] += 0.5  # 중력 효과 적용 (아래로 내려감)

        ball_val["pos_x"] += ball_val["to_x"]  # x 좌표 업데이트
        ball_val["pos_y"] += ball_val["to_y"]  # y 좌표 업데이트

    return balls

def check_collisions(character_x_pos, character_y_pos, character, balls, weapons, weapon_to_remove, ball_to_remove, game_result, running, ball_speed_y):
    """충돌 처리"""
    character_rect = character.get_rect()
    character_rect.left = character_x_pos
    character_rect.top = character_y_pos

    # 캐릭터 높이 업데이트 (캐릭터 이미지가 변경될 수 있으므로)
    character_width = character_rect.size[0]

    for ball_idx, ball_val in enumerate(balls):
        ball_pos_x = ball_val["pos_x"]
        ball_pos_y = ball_val["pos_y"]
        ball_img_idx = ball_val["img_idx"]

        ball_rect = ball_images[ball_img_idx].get_rect()
        ball_rect.left = ball_pos_x
        ball_rect.top = ball_pos_y

        # 캐릭터와 공의 충돌 체크
        if character_rect.colliderect(ball_rect):
            running = False  # 게임 종료
            break

        # 무기와 공의 충돌 체크
        for weapon_idx, weapon_val in enumerate(weapons):
            weapon_pos_x = weapon_val[0]
            weapon_pos_y = weapon_val[1]

            weapon_rect = weapon.get_rect()
            weapon_rect.left = weapon_pos_x
            weapon_rect.top = weapon_pos_y

            # 충돌 발생 시
            if weapon_rect.colliderect(ball_rect):
                weapon_to_remove = weapon_idx  # 해당 무기 제거 설정
                ball_to_remove = ball_idx  # 해당 공 제거 설정

                # 가장 작은 공이 아니라면 공 분열 처리
                if ball_img_idx < 3:
                    split_ball(balls, ball_val, ball_img_idx, ball_speed_y)
                break
        else:
            continue
        break

    # 충돌된 무기 제거
    if weapon_to_remove > -1:
        del weapons[weapon_to_remove]
        weapon_to_remove = -1

    # 충돌된 공 제거
    if ball_to_remove > -1:
        del balls[ball_to_remove]
        ball_to_remove = -1

    return weapon_to_remove, ball_to_remove, game_result, running

def split_ball(balls, ball_val, ball_img_idx, ball_speed_y):
    """공 분열 처리"""
    ball_width = ball_images[ball_img_idx].get_rect().size[0]
    ball_height = ball_images[ball_img_idx].get_rect().size[1]

    small_ball_img_idx = ball_img_idx + 1  # 다음 크기의 공 이미지 사용
    small_ball_size = ball_images[small_ball_img_idx].get_rect().size
    small_ball_width = small_ball_size[0]
    small_ball_height = small_ball_size[1]

    # 왼쪽으로 튕겨나가는 작은 공
    balls.append({
        "pos_x": ball_val["pos_x"] + (ball_width / 2) - (small_ball_width / 2),
        "pos_y": ball_val["pos_y"] + (ball_height / 2) - (small_ball_height / 2),
        "img_idx": small_ball_img_idx,
        "to_x": -3,  # 왼쪽으로 이동
        "to_y": -6,  # 살짝 위로 이동
        "init_spd_y": ball_speed_y[small_ball_img_idx],  # 해당 크기 공의 초기 속도
    })

    # 오른쪽으로 튕겨나가는 작은 공
    balls.append({
        "pos_x": ball_val["pos_x"] + (ball_width / 2) - (small_ball_width / 2),
        "pos_y": ball_val["pos_y"] + (ball_height / 2) - (small_ball_height / 2),
        "img_idx": small_ball_img_idx,
        "to_x": 3,  # 오른쪽으로 이동
        "to_y": -6,
        "init_spd_y": ball_speed_y[small_ball_img_idx],
    })

def draw_screen(character_x_pos, character_y_pos, character, weapons, balls):
    """화면에 그리기"""
    # 배경 이미지 그리기
    screen.blit(pygame.image.load(os.path.join(image_path, "게임배경.jpg")), (0, 0))

    # 무기 그리기
    for weapon_x_pos, weapon_y_pos in weapons:
        screen.blit(weapon, (weapon_x_pos, weapon_y_pos))

    # 공 그리기
    for ball_val in balls:
        ball_pos_x = ball_val["pos_x"]
        ball_pos_y = ball_val["pos_y"]
        ball_img_idx = ball_val["img_idx"]
        screen.blit(ball_images[ball_img_idx], (ball_pos_x, ball_pos_y))

    # 스테이지 그리기
    screen.blit(stage, (0, screen_height - stage.get_rect().size[1]))

    # 캐릭터 그리기
    screen.blit(character, (character_x_pos, character_y_pos))

    # 무기 발사 시 weapon_y_pos 업데이트
    for idx, weapon_val in enumerate(weapons):
        if weapon_val[1] == 0:
            # 캐릭터의 현재 y 위치에서 weapon_y_pos 설정
            weapons[idx][1] = character_y_pos

def draw_timer(total_time, elapsed_time):
    """타이머 그리기"""
    timer = font.render(f"Time : {int(total_time - elapsed_time)}", True, (255, 255, 255))
    screen.blit(timer, (10, 10))

def show_game_over(game_result, now_stop):
    """게임 종료 메시지 표시"""
    if not now_stop:
        msg = font.render(game_result, True, (255, 255, 0))  # 노란색 텍스트
        msg_rect = msg.get_rect(center=(int(screen_width / 2), int(screen_height / 2)))  # 화면 중앙에 메시지 표시
        screen.blit(msg, msg_rect)
        pygame.display.update()
        pygame.time.delay(2000)  # 2초 대기
    pygame.quit()  # pygame 종료

if __name__ == "__main__":
    main()
