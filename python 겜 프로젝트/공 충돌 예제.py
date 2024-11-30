import math,random,sys
import pygame   
from pygame.locals import*

def events():
    for event in pygame.event.get():
        if event.type == pygame.QUIT or (event.type == pygame.KEYDOWN and event.key == pygame.K_SPACE):
            pygame.quit()
            sys.exit()

W = 500
H = 480
HW, HH = W/2, H/2
AREA = W * H 

# 시작
pygame.init()
# FPS
CLOCK = pygame.time.Clock()
# 화면 크기
DS = pygame.display.set_mode((W, H))
# 게임 이름
pygame.display.set_caption("circle collision")
# FPS 설정
FPS = 120

# 색깔
BLACK = (  0,   0,   0)
WHITE = (255, 255, 255)
BLUE  = (  0,   0, 255)
GREEN = (  0, 255,   0)
RED   = (255,   0,   0)

x1,y1, radius1,color1 = HW,HH,50,None
x2,y2, radius2,color2 = None,None,50,WHITE

while True:
    events()

    x2,y2 = pygame.mouse.get_pos()

    distance = math.hypot(x1 - x2, y1 -y2)
    if distance <= radius1 + radius2:
        color1 = RED

    else: 
        color1 = GREEN

    pygame.draw.circle(DS, color1, (x1,y1), radius1, 0 )
    pygame.draw.circle(DS, color2, (x2,y2), radius2, 0 )

    pygame.display.update()
    CLOCK.tick(FPS)
    DS.fill(BLACK)