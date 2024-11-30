# character_to_x_right 과 character_to_x_left  변수를 방향마다 따로 지정하듯이
# left_pressed 와 right_pressed를 False로 지정합니다 (boolean)


character_to_x_LEFT = 0
character_to_x_RIGHT= 0 

if event.type == pygame.KEYDOWN:
    if event.key == pygame.K_LEFT:
        character_to_x_LEFT -= character_speed
        left_pressed = True
        if right_pressed:
            character_to_x_RIGHT = 0
            right_pressed = False
    elif event.key == pygame.K_RIGHT:
        character_to_x_RIGHT += character_speed
        right_pressed = True
        if left_pressed:
            character_to_x_LEFT = 0
            left_pressed = False 

if event.type == pygame.KEYUP:
    if event.key == pygame.K_LEFT:
        character_to_x_LEFT = 0
        left_pressed = False
    elif event.key == pygame.K_RIGHT:
        character_to_x_RIGHT = 0
        right_pressed = False 

character_x_pos += character_to_x_LEFT + character_to_x_RIGHT

# 앞서 논의했듯이 좌우키를 동시에 누르거나, 한쪽키를 누르고 있는 와중에 다른 키를 누르면 멈추는 현상이 생기는데, 이건 한 프레임에 코드가 event.key가 K.LEFT 와 K.RIGHT를 둘다 인식해서 최종적으로 to_x 방향이 0으로 합산되는 현상인데요, 이걸 개선할려면 (right,left)_pressed 변수를 스위치 용도로 사용해서 만약 한 방향을 누를때 다른 방향이 눌러져 있으면 (pressed 변수로 확인) 다른 방향의 속도를 0으로 만들고 다른 방향의 키가 눌러져 있어도 안눌러져있는 것 처럼 인식하게 만드는 방법입니다.
# 예: 왼쪽을 누르고 있는대 오른쪽을 누르면, 오른쪽을 누름으로서 왼쪽이 눌러져 있다는 스위치를 끄고, 왼쪽 속도를 0으로 만든다. 
# 이 코드가 가능한 이유는 우리가 다시 왼쪽으로 방향을 변경하고 싶을떄는 무조건 손가락으로 왼쪽 방향키에 KEYUP과 KEYDOWN을 다시 눌러야해서 그럽니다. (그러면 left_pressed가 다시 켜지고 right_pressed스위치가 꺼짐으로써 아무리 오른쪽 버튼을 누르고 있었던 상황임에도 왼쪽으로 방향을 틀겠죠?)

# 한번 사용해보세요 엄청 부드럽습니다 :)