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
character_y_pos = 0  # 캐릭터의 y 위치

# 점수 변수 추가
score = 0  # 현재 점수

def init_game():
    """게임 초기화 및 설정"""
    global screen, clock, current_path, image_path, character_images, ball_images, weapon, stage, font

    pygame.init()  # Pygame 라이브러리 초기화
    screen = pygame.display.set_mode((screen_width, screen_height))  # 화면 설정
    pygame.display.set_caption("Pang Game")  # 게임 제목 설정
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

def show_title_screen():
    """타이틀 화면 표시"""
    title_font = pygame.font.Font(None, 74)
    title_text = title_font.render("Pang Game", True, (255, 255, 255))
    start_font = pygame.font.Font(None, 50)
    start_text = start_font.render("Press SPACE to Start", True, (255, 255, 255))

    while True:
        screen.fill((0, 0, 0))  # 배경을 검은색으로 채움
        screen.blit(title_text, (screen_width / 2 - title_text.get_width() / 2, screen_height / 2 - 100))
        screen.blit(start_text, (screen_width / 2 - start_text.get_width() / 2, screen_height / 2 + 50))
        pygame.display.update()

        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                pygame.quit()
                exit()
            elif event.type == pygame.KEYDOWN:
                if event.key == pygame.K_SPACE:
                    return  # 메인 게임으로 이동

def main():
    """메인 게임 루프"""
    global character_to_x, character, score, character_y_pos  # 전역 변수 선언
    init_game()  # 게임 초기화 함수 호출

    # 타이틀 화면 표시
    show_title_screen()

    # 캐릭터에 대한 초기 설정
    character = character_images["default"]
    character_width = character.get_rect().size[0]
    character_height = character.get_rect().size[1]
    stage_height = stage.get_rect().size[1]
    character_x_pos = (screen_width / 2) - (character_width / 2)
    character_y_pos = screen_height - character_height - stage_height
    character_speed = 5  # 캐릭터의 이동 속도
    dash_speed = 5  # 대시 시 추가 속도

    # 무기에 대한 초기 설정
    weapon_width = weapon.get_rect().size[0]
    weapons = []
    weapon_speed = 10

    # 공에 대한 초기 설정
    ball_speed_y = [-18, -15, -12, -9]
    balls = []
    balls.append({
        "pos_x": 50,
        "pos_y": 50,
        "img_idx": 0,
        "to_x": 3,
        "to_y": -6,
        "init_spd_y": ball_speed_y[0],
    })

    weapon_to_remove = -1
    ball_to_remove = -1

    total_time = 100
    start_ticks = pygame.time.get_ticks()
    game_result = "Game Over"
    now_stop = False

    running = True
    while running:
        dt = clock.tick(30)

        # 캐릭터 위치 업데이트 (이벤트 처리 전에)
        character_x_pos = update_character_position(character_x_pos, character_to_x, character_width)

        # 이벤트 처리
        running, now_stop, weapons = handle_events(
            character_speed, dash_speed, character_images, weapons, character_x_pos, character_width, weapon_width
        )

        # 무기 위치 업데이트
        weapons = update_weapons(weapons, weapon_speed)

        # 공 위치 업데이트
        balls = update_balls(balls, ball_speed_y)

        # 충돌 처리
        weapon_to_remove, ball_to_remove, game_result, running = check_collisions(
            character_x_pos, character_y_pos, character, balls, weapons, weapon_to_remove, ball_to_remove, game_result, running, ball_speed_y
        )

        if len(balls) == 0:
            game_result = "Mission Complete"
            running = False

        # 화면에 그리기
        draw_screen(character_x_pos, character_y_pos, character, weapons, balls)

        # 경과 시간 계산
        elapsed_time = (pygame.time.get_ticks() - start_ticks) / 1000

        if total_time - elapsed_time <= 0:
            game_result = "Time Over"
            running = False

        # 타이머 그리기
        draw_timer(total_time, elapsed_time)

        # 점수 표시
        draw_score(score)

        pygame.display.update()

    show_game_over(game_result, now_stop)

def handle_events(character_speed, dash_speed, character_images, weapons, character_x_pos, character_width, weapon_width):
    """이벤트 처리"""
    global character_to_x, left_pressed, right_pressed, character, character_y_pos  # 전역 변수 선언

    now_stop = False
    running = True

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            now_stop = True
            running = False

        if event.type == pygame.KEYDOWN:
            if event.key == pygame.K_LEFT:
                character_to_x = -character_speed
                left_pressed = True
                character = character_images["left"]
            if event.key == pygame.K_RIGHT:
                character_to_x = character_speed
                right_pressed = True
                character = character_images["right"]
            if event.key == pygame.K_z:
                character_to_x = -character_speed - dash_speed
                character = character_images["left"]
            if event.key == pygame.K_x:
                character_to_x = character_speed + dash_speed
                character = character_images["right"]
            if event.key == pygame.K_DOWN:
                character = character_images["down"]
                character_to_x = 0
            if event.key == pygame.K_SPACE:
                character = character_images["attack"]
                weapon_x_pos = character_x_pos + (character_width / 2) - (weapon_width / 2)
                weapon_y_pos = character_y_pos  # 업데이트된 캐릭터 y 위치 사용
                weapons.append([weapon_x_pos, weapon_y_pos])

        if event.type == pygame.KEYUP:
            if event.key == pygame.K_LEFT and left_pressed:
                character_to_x = 0
                left_pressed = False
                character = character_images["default"]
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
    global character_y_pos  # 전역 변수 사용
    character_x_pos += character_to_x

    if character_x_pos < 0:
        character_x_pos = 0
    elif character_x_pos > screen_width - character_width:
        character_x_pos = screen_width - character_width

    character_height = character.get_rect().size[1]
    stage_height = stage.get_rect().size[1]
    character_y_pos = screen_height - character_height - stage.get_rect().size[1]

    return character_x_pos

def update_weapons(weapons, weapon_speed):
    """무기 위치 업데이트"""
    weapons = [[w[0], w[1] - weapon_speed] for w in weapons]
    weapons = [ [w[0], w[1]] for w in weapons if w[1] > 0]
    return weapons

def update_balls(balls, ball_speed_y):
    """공 위치 업데이트"""
    for ball_val in balls:
        ball_pos_x = ball_val["pos_x"]
        ball_pos_y = ball_val["pos_y"]
        ball_img_idx = ball_val["img_idx"]

        ball_size = ball_images[ball_img_idx].get_rect().size
        ball_width = ball_size[0]
        ball_height = ball_size[1]

        if ball_pos_x <= 0 or ball_pos_x >= screen_width - ball_width:
            ball_val["to_x"] *= -1

        if ball_pos_y >= screen_height - stage.get_rect().size[1] - ball_height:
            ball_val["to_y"] = ball_val["init_spd_y"]
        else:
            ball_val["to_y"] += 0.5

        ball_val["pos_x"] += ball_val["to_x"]
        ball_val["pos_y"] += ball_val["to_y"]

    return balls

def check_collisions(character_x_pos, character_y_pos, character, balls, weapons, weapon_to_remove, ball_to_remove, game_result, running, ball_speed_y):
    """충돌 처리"""
    global score

    character_rect = character.get_rect()
    character_rect.left = character_x_pos
    character_rect.top = character_y_pos

    character_width = character_rect.size[0]

    for ball_idx, ball_val in enumerate(balls):
        ball_pos_x = ball_val["pos_x"]
        ball_pos_y = ball_val["pos_y"]
        ball_img_idx = ball_val["img_idx"]

        ball_rect = ball_images[ball_img_idx].get_rect()
        ball_rect.left = ball_pos_x
        ball_rect.top = ball_pos_y

        if character_rect.colliderect(ball_rect):
            running = False
            break

        for weapon_idx, weapon_val in enumerate(weapons):
            weapon_pos_x = weapon_val[0]
            weapon_pos_y = weapon_val[1]

            weapon_rect = weapon.get_rect()
            weapon_rect.left = weapon_pos_x
            weapon_rect.top = weapon_pos_y

            if weapon_rect.colliderect(ball_rect):
                weapon_to_remove = weapon_idx
                ball_to_remove = ball_idx

                score += (4 - ball_img_idx) * 10

                if ball_img_idx < 3:
                    split_ball(balls, ball_val, ball_img_idx, ball_speed_y)
                break
        else:
            continue
        break

    if weapon_to_remove > -1:
        del weapons[weapon_to_remove]
        weapon_to_remove = -1

    if ball_to_remove > -1:
        del balls[ball_to_remove]
        ball_to_remove = -1

    return weapon_to_remove, ball_to_remove, game_result, running

def split_ball(balls, ball_val, ball_img_idx, ball_speed_y):
    """공 분열 처리"""
    ball_width = ball_images[ball_img_idx].get_rect().size[0]
    ball_height = ball_images[ball_img_idx].get_rect().size[1]

    small_ball_img_idx = ball_img_idx + 1
    small_ball_size = ball_images[small_ball_img_idx].get_rect().size
    small_ball_width = small_ball_size[0]
    small_ball_height = small_ball_size[1]

    balls.append({
        "pos_x": ball_val["pos_x"] + (ball_width / 2) - (small_ball_width / 2),
        "pos_y": ball_val["pos_y"] + (ball_height / 2) - (small_ball_height / 2),
        "img_idx": small_ball_img_idx,
        "to_x": -3,
        "to_y": -6,
        "init_spd_y": ball_speed_y[small_ball_img_idx],
    })

    balls.append({
        "pos_x": ball_val["pos_x"] + (ball_width / 2) - (small_ball_width / 2),
        "pos_y": ball_val["pos_y"] + (ball_height / 2) - (small_ball_height / 2),
        "img_idx": small_ball_img_idx,
        "to_x": 3,
        "to_y": -6,
        "init_spd_y": ball_speed_y[small_ball_img_idx],
    })

def draw_screen(character_x_pos, character_y_pos, character, weapons, balls):
    """화면에 그리기"""
    screen.blit(pygame.image.load(os.path.join(image_path, "게임배경.jpg")), (0, 0))

    for weapon_x_pos, weapon_y_pos in weapons:
        screen.blit(weapon, (weapon_x_pos, weapon_y_pos))

    for ball_val in balls:
        ball_pos_x = ball_val["pos_x"]
        ball_pos_y = ball_val["pos_y"]
        ball_img_idx = ball_val["img_idx"]
        screen.blit(ball_images[ball_img_idx], (ball_pos_x, ball_pos_y))

    screen.blit(stage, (0, screen_height - stage.get_rect().size[1]))

    screen.blit(character, (character_x_pos, character_y_pos))

def draw_timer(total_time, elapsed_time):
    """타이머 그리기"""
    timer = font.render(f"Time : {int(total_time - elapsed_time)}", True, (255, 255, 255))
    screen.blit(timer, (10, 10))

def draw_score(score):
    """점수 표시"""
    score_display = font.render(f"Score : {score}", True, (255, 255, 255))
    screen.blit(score_display, (10, 50))

def show_game_over(game_result, now_stop):
    """게임 종료 메시지 표시"""
    if not now_stop:
        msg = font.render(game_result, True, (255, 255, 0))
        msg_rect = msg.get_rect(center=(int(screen_width / 2), int(screen_height / 2)))
        screen.blit(msg, msg_rect)
        pygame.display.update()
        pygame.time.delay(2000)
    pygame.quit()

if __name__ == "__main__":
    main()
