# pygame 게임의 기본적인 게임 툴, 만들때 이것을 사용하면 좋음
import pygame 
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
class Block(pygame.sprite.Sprite):
    def __init__(self,img):
        super().__init__()
        self.image = img
        self.rect = img.get_rect()

pic = pygame.image.load("C:/Users/kimbu/Desktop/파이썬실습용/MY 프로젝트/겜 프로젝트/이미지/축구공.png").convert_alpha()
pic2 = pygame.transform.scale(pic,(200,200))
pic3 = pygame.transform.scale(pic,(100,100))

pic = Block(pic)
pic.rect.center(400,400) 
pic.radius =100
# pic2.radius=100
# pic3.get_rect()
# pic3.radius=100


# b1 = Block(pic2)
# b1.rect.center=(400,400)
# b1.radius=100

# b2 = Block(pic3)
# b1.rect.center=(400,400)
# b2.radius = 50


running=True 
while running:  
    dt = clock.tick(30) 

    # 2. 이벤트 처리 (키보드, 마우스 등)                     
    for event in pygame.event.get():
        if event.type == pygame.QUIT:  
            running = False 
        
    # 3. 게임 캐릭터 위치 정의

    # 4. 충돌 처리
    if(pygame.sprite.collide_circle(b1,b2)):
        print("Hit!")
    # 5. 화면에 그리기
    surface.blit(self.image,self.rect)
    pygame.display.update() 


pygame.quit()

