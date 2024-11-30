import pygame 
pic = pygame.image.load("C:/Users/kimbu/Desktop/파이썬실습용/MY 프로젝트/겜 프로젝트/이미지/축구공.png").convert_alpha()
pic2 = pygame.transform.scale(pic,(200,200))
pic3 = pygame.transform.scale(pic,(100,100))

b1 = Block(pic2)
b1.rect.center=(400,400)
b1.radius=100

b2 = Block(pic3)
b2.radius = 50


# 사각형 충돌
if(pygame.sprite.collide_rect(b1,b2)):
    print("Hit!")
# 원 충돌
if(pygame.sprite.collide_circle(b1,b2)):
    print("Hit!")

# mask
b1.mask = pygame.mask.from_surface(b1.image)
b2.mask = pygame.mask.from_surface(b2.image)
if(pygame.sprite.collide_mask(b1,b2)):
	print("Hit!")

pic = pygame.image.load("images/ball.png").convert_alpha()




pic = []
pic.append(pygame.image.load("images/bb.png").convert_alpha())
pic.append(pygame.image.load("images/gb.png").convert_alpha())
pic.append(pygame.image.load("images/pb.png").convert_alpha())
pic.append(pygame.image.load("images/rb.png").convert_alpha())
pic.append(pygame.image.load("images/yb.png").convert_alpha())

block_list = pygame.sprite.Group()

for j in range(0,5):
    for i in range(1,9):    
        block = Block(pic[random.randrange(5)])
        block.rect.x = i * 91 - 50
        block.rect.y = 50 + 37*j
        block.mask = pygame.mask.from_surface(block.image)
        block_list.add(block)

ball_pic = pygame.image.load("images/ball.png").convert_alpha()
ball_pic = pygame.transform.scale(ball_pic,(15,15))
ball = Block(ball_pic)
ball.rect.center = (410,680)
ball.mask = pygame.mask.from_surface(ball.image)

hit_list = pygame.sprite.spritecollide(ball,block_list,True,pygame.sprite.collide_mask)
for h in hit_list:
    score +=1