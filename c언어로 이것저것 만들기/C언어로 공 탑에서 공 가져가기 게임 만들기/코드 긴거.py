#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <string.h>
#include <ctype.h>

#define BOARD_SIZE 5
#define MAX_TURNS 15
#define PLAYER_NAME_MAX 30
#define INPUT_BUFFER_SIZE 100
#define MAX_INPUT_ATTEMPTS 3
#define EMPTY_CELL 'O'
#define PLAYER1_SYMBOL 'X'
#define PLAYER2_SYMBOL '@'
#define PLAYER1_ID 1
#define PLAYER2_ID 2
#define FALSE 0
#define TRUE 1
#define SUCCESS 0
#define FAILURE -1
#define INPUT_ERROR -2
#define BUFFER_OVERFLOW -3
#define MAX_INT_VALUE 2147483647
#define MIN_INT_VALUE -2147483648

static char board[BOARD_SIZE][BOARD_SIZE];
static char player1_name[PLAYER_NAME_MAX];
static char player2_name[PLAYER_NAME_MAX];

static void initialize_board(void);
static void display_board(void);
static int get_player_names(void);
static int get_first_player_choice(void);
static int validate_input_range(int level, int b_pos, int b_value);
static int check_cell_availability(int level, int b_pos, int b_value);
static void place_symbols(int level, int b_pos, int b_value, int current_turn);
static int count_remaining_cells(void);
static void display_game_result(int current_turn);
static void display_player_info(void);
static int get_player_input(int current_turn, int* level, int* b_pos, int* b_value);
static void display_separator(void);
static void display_game_header(void);
static int validate_level(int level);
static int validate_b_pos(int level, int b_pos);
static int validate_b_value(int level, int b_pos, int b_value);
static void clear_input_buffer(void);
static int safe_string_input(char* buffer, int max_size);
static int safe_integer_input(int* value, int min_range, int max_range);
static int safe_three_integers_input(int* val1, int* val2, int* val3);
static int validate_string_input(const char* input);
static int is_valid_integer_string(const char* str);
static void display_input_error_message(int error_code);
static int handle_input_retry(int attempt_count);
static void flush_stdin(void);
static int custom_string_to_int(const char* str, int* result);
static int custom_power(int base, int exp);
static void skip_whitespace(const char** str);
static int split_string_by_space(const char* input, char tokens[][20], int max_tokens);

int main(void)
{
    int turn_counter = 0;
    int level = 0;
    int b_pos = 0;
    int b_value = 0;
    int current_turn = FALSE;
    int first_player_choice = 0;
    int input_result = SUCCESS;
    int validation_result = SUCCESS;
    int remaining_cells = 0;

    display_game_header();

    if (get_player_names() != SUCCESS)
    {
        printf("플레이어 이름 입력에 실패했습니다. 게임을 종료합니다.\n");
        return FAILURE;
    }

    first_player_choice = get_first_player_choice();
    if (first_player_choice == PLAYER1_ID)
    {
        current_turn = FALSE;
    }
    else if (first_player_choice == PLAYER2_ID)
    {
        current_turn = TRUE;
    }
    else
    {
        current_turn = FALSE;
        printf("잘못된 선택입니다. 플레이어 1이 먼저 시작합니다.\n");
    }

    initialize_board();
    display_board();

    for (turn_counter = 0; turn_counter < MAX_TURNS; turn_counter++)
    {
        display_player_info();

        input_result = get_player_input(current_turn, &level, &b_pos, &b_value);
        if (input_result != SUCCESS)
        {
            printf("입력 처리 중 오류가 발생했습니다. 다시 시도해주세요.\n");
            turn_counter--;
            continue;
        }

        validation_result = validate_input_range(level, b_pos, b_value);
        if (validation_result != SUCCESS)
        {
            turn_counter--;
            continue;
        }

        validation_result = check_cell_availability(level, b_pos, b_value);
        if (validation_result != SUCCESS)
        {
            turn_counter--;
            continue;
        }

        place_symbols(level, b_pos, b_value, current_turn);
        display_board();

        remaining_cells = count_remaining_cells();
        if (remaining_cells == 0)
        {
            display_game_result(current_turn);
            return SUCCESS;
        }
        else
        {
            printf("<<남은 공 개수 : %d 개>>\n", remaining_cells);
        }

        current_turn = (current_turn == FALSE) ? TRUE : FALSE;
    }

    printf("최대 턴 수에 도달했습니다. 게임을 종료합니다.\n");
    return SUCCESS;
}

static int custom_power(int base, int exp)
{
    int result = 1;
    int i = 0;

    if (exp < 0)
    {
        return 0;
    }

    for (i = 0; i < exp; i++)
    {
        result *= base;
    }

    return result;
}

static void skip_whitespace(const char** str)
{
    if (str == NULL || *str == NULL)
    {
        return;
    }

    while (**str == ' ' || **str == '\t' || **str == '\n' || **str == '\r')
    {
        (*str)++;
    }
}

static int custom_string_to_int(const char* str, int* result)
{
    int value = 0;
    int sign = 1;
    int digit = 0;
    int i = 0;
    int len = 0;
    const char* current = str;

    if (str == NULL || result == NULL)
    {
        return FAILURE;
    }

    skip_whitespace(&current);

    if (*current == '\0')
    {
        return FAILURE;
    }

    if (*current == '-')
    {
        sign = -1;
        current++;
    }
    else if (*current == '+')
    {
        current++;
    }

    if (*current == '\0')
    {
        return FAILURE;
    }

    len = (int)strlen(current);

    if (len > 10)
    {
        return FAILURE;
    }

    for (i = 0; current[i] != '\0'; i++)
    {
        if (!isdigit((unsigned char)current[i]))
        {
            return FAILURE;
        }

        digit = current[i] - '0';

        if (value > (MAX_INT_VALUE - digit) / 10)
        {
            return FAILURE;
        }

        value = value * 10 + digit;
    }

    *result = value * sign;

    if (sign == -1 && value > (MAX_INT_VALUE + 1))
    {
        return FAILURE;
    }

    return SUCCESS;
}

static int split_string_by_space(const char* input, char tokens[][20], int max_tokens)
{
    int token_count = 0;
    int char_index = 0;
    int input_index = 0;
    int input_len = 0;

    if (input == NULL || tokens == NULL || max_tokens <= 0)
    {
        return 0;
    }

    input_len = (int)strlen(input);

    while (input_index < input_len && token_count < max_tokens)
    {
        while (input_index < input_len && (input[input_index] == ' ' || input[input_index] == '\t'))
        {
            input_index++;
        }

        if (input_index >= input_len)
        {
            break;
        }

        char_index = 0;
        while (input_index < input_len &&
            input[input_index] != ' ' &&
            input[input_index] != '\t' &&
            char_index < 19)
        {
            tokens[token_count][char_index] = input[input_index];
            char_index++;
            input_index++;
        }

        tokens[token_count][char_index] = '\0';

        if (char_index > 0)
        {
            token_count++;
        }
    }

    return token_count;
}

static void clear_input_buffer(void)
{
    int c = 0;
    while ((c = getchar()) != '\n' && c != EOF)
    {
    }
}

static void flush_stdin(void)
{
    int c = 0;
    do
    {
        c = getchar();
    } while (c != '\n' && c != EOF);
}

static int safe_string_input(char* buffer, int max_size)
{
    char temp_buffer[INPUT_BUFFER_SIZE];
    int len = 0;
    int attempt = 0;

    if (buffer == NULL || max_size <= 0)
    {
        return FAILURE;
    }

    for (attempt = 0; attempt < MAX_INPUT_ATTEMPTS; attempt++)
    {
        if (fgets(temp_buffer, INPUT_BUFFER_SIZE, stdin) != NULL)
        {
            len = (int)strlen(temp_buffer);
            if (len > 0 && temp_buffer[len - 1] == '\n')
            {
                temp_buffer[len - 1] = '\0';
                len--;
            }

            if (len >= max_size)
            {
                printf("입력이 너무 깁니다. 최대 %d자까지 입력 가능합니다.\n", max_size - 1);
                continue;
            }

            if (validate_string_input(temp_buffer) == SUCCESS)
            {
                {
                    int i = 0;
                    int copy_len = (len < max_size - 1) ? len : max_size - 1;
                    for (i = 0; i < copy_len; i++)
                    {
                        buffer[i] = temp_buffer[i];
                    }
                    buffer[copy_len] = '\0';
                }
                return SUCCESS;
            }
            else
            {
                printf("잘못된 문자가 포함되어 있습니다. 영문자와 숫자만 입력해주세요.\n");
            }
        }
        else
        {
            printf("입력 읽기 오류가 발생했습니다.\n");
            clear_input_buffer();
        }

        if (handle_input_retry(attempt + 1) != SUCCESS)
        {
            break;
        }
    }

    return FAILURE;
}

static int validate_string_input(const char* input)
{
    int i = 0;
    int len = 0;

    if (input == NULL)
    {
        return FAILURE;
    }

    len = (int)strlen(input);
    if (len == 0)
    {
        printf("빈 문자열은 입력할 수 없습니다.\n");
        return FAILURE;
    }

    for (i = 0; i < len; i++)
    {
        if (!isalnum((unsigned char)input[i]) && input[i] != ' ')
        {
            if ((unsigned char)input[i] < 128)
            {
                return FAILURE;
            }
        }
    }

    return SUCCESS;
}

static int safe_integer_input(int* value, int min_range, int max_range)
{
    char input_buffer[INPUT_BUFFER_SIZE];
    int attempt = 0;
    int result = 0;

    if (value == NULL)
    {
        return FAILURE;
    }

    for (attempt = 0; attempt < MAX_INPUT_ATTEMPTS; attempt++)
    {
        printf("정수를 입력하세요 (%d ~ %d): ", min_range, max_range);

        if (fgets(input_buffer, INPUT_BUFFER_SIZE, stdin) != NULL)
        {
            {
                int len = (int)strlen(input_buffer);
                if (len > 0 && input_buffer[len - 1] == '\n')
                {
                    input_buffer[len - 1] = '\0';
                }
            }

            if (custom_string_to_int(input_buffer, &result) == SUCCESS)
            {
                if (result >= min_range && result <= max_range)
                {
                    *value = result;
                    return SUCCESS;
                }
                else
                {
                    printf("입력값이 범위를 벗어났습니다. (%d ~ %d)\n", min_range, max_range);
                }
            }
            else
            {
                printf("유효한 정수가 아닙니다.\n");
            }
        }
        else
        {
            printf("입력 읽기 오류가 발생했습니다.\n");
            clear_input_buffer();
        }

        if (handle_input_retry(attempt + 1) != SUCCESS)
        {
            break;
        }
    }

    return FAILURE;
}

static int is_valid_integer_string(const char* str)
{
    int i = 0;
    int start_index = 0;

    if (str == NULL || strlen(str) == 0)
    {
        return FALSE;
    }

    if (str[0] == '+' || str[0] == '-')
    {
        start_index = 1;
        if (strlen(str) == 1)
        {
            return FALSE;
        }
    }

    for (i = start_index; str[i] != '\0'; i++)
    {
        if (!isdigit((unsigned char)str[i]))
        {
            return FALSE;
        }
    }

    return TRUE;
}

static int safe_three_integers_input(int* val1, int* val2, int* val3)
{
    char input_buffer[INPUT_BUFFER_SIZE];
    char tokens[10][20];
    int token_count = 0;
    int values[3];
    int attempt = 0;
    int i = 0;

    if (val1 == NULL || val2 == NULL || val3 == NULL)
    {
        return FAILURE;
    }

    for (attempt = 0; attempt < MAX_INPUT_ATTEMPTS; attempt++)
    {
        if (fgets(input_buffer, INPUT_BUFFER_SIZE, stdin) != NULL)
        {
            {
                int len = (int)strlen(input_buffer);
                if (len > 0 && input_buffer[len - 1] == '\n')
                {
                    input_buffer[len - 1] = '\0';
                }
            }

            token_count = split_string_by_space(input_buffer, tokens, 10);

            if (token_count == 3)
            {
                for (i = 0; i < 3; i++)
                {
                    if (custom_string_to_int(tokens[i], &values[i]) != SUCCESS)
                    {
                        printf("'%s'는 유효한 정수가 아닙니다.\n", tokens[i]);
                        break;
                    }
                }

                if (i == 3)
                {
                    *val1 = values[0];
                    *val2 = values[1];
                    *val3 = values[2];
                    return SUCCESS;
                }
            }
            else if (token_count < 3)
            {
                printf("3개의 정수가 필요합니다. (현재 입력: %d개)\n", token_count);
            }
            else
            {
                printf("너무 많은 값이 입력되었습니다. 3개만 입력해주세요.\n");
            }
        }
        else
        {
            printf("입력 읽기 오류가 발생했습니다.\n");
            clear_input_buffer();
        }

        if (handle_input_retry(attempt + 1) != SUCCESS)
        {
            break;
        }
    }

    return FAILURE;
}

static int handle_input_retry(int attempt_count)
{
    if (attempt_count >= MAX_INPUT_ATTEMPTS)
    {
        printf("최대 시도 횟수를 초과했습니다.\n");
        return FAILURE;
    }

    printf("다시 시도해주세요. (시도 %d/%d)\n", attempt_count + 1, MAX_INPUT_ATTEMPTS);
    return SUCCESS;
}

static void display_input_error_message(int error_code)
{
    switch (error_code)
    {
    case INPUT_ERROR:
        printf("입력 형식이 올바르지 않습니다.\n");
        break;
    case BUFFER_OVERFLOW:
        printf("입력이 너무 깁니다.\n");
        break;
    default:
        printf("알 수 없는 오류가 발생했습니다.\n");
        break;
    }
}

static void initialize_board(void)
{
    int i = 0;
    int j = 0;

    for (i = 0; i < BOARD_SIZE; i++)
    {
        for (j = 0; j < BOARD_SIZE; j++)
        {
            board[i][j] = '\0';
        }
    }

    for (i = 0; i < BOARD_SIZE; i++)
    {
        for (j = 0; j < (i + 1); j++)
        {
            board[i][j] = EMPTY_CELL;
        }
    }
}

static void display_board(void)
{
    int n = 0;
    int m = 0;
    int b = 0;

    display_separator();

    for (n = 0; n < BOARD_SIZE; n++)
    {
        for (m = (BOARD_SIZE - 1) - n; m > 0; m--)
        {
            printf(" ");
        }

        for (b = 0; b < (n + 1); b++)
        {
            printf("%c ", board[n][b]);
        }
        printf("\n");
    }

    display_separator();
}

static int get_player_names(void)
{
    printf("플레이어 P1 이름을 입력하세요: ");
    flush_stdin();

    if (safe_string_input(player1_name, PLAYER_NAME_MAX) != SUCCESS)
    {
        printf("플레이어 P1 이름 입력에 실패했습니다.\n");
        return FAILURE;
    }

    printf("플레이어 P2 이름을 입력하세요: ");
    if (safe_string_input(player2_name, PLAYER_NAME_MAX) != SUCCESS)
    {
        printf("플레이어 P2 이름 입력에 실패했습니다.\n");
        return FAILURE;
    }

    return SUCCESS;
}

static int get_first_player_choice(void)
{
    int choice = 0;

    printf("누가 먼저 하실 건가요?(1 : P1, 2: P2 가 선공)\n");

    if (safe_integer_input(&choice, 1, 2) == SUCCESS)
    {
        return choice;
    }

    printf("입력에 실패했습니다. 플레이어 1이 먼저 시작합니다.\n");
    return PLAYER1_ID;
}

static int get_player_input(int current_turn, int* level, int* b_pos, int* b_value)
{
    if (current_turn == FALSE)
    {
        printf("%s 님의 값을 입력해주세요(level b_pos b_value) => ", player1_name);
    }
    else
    {
        printf("%s 님의 값을 입력해주세요(level b_pos b_value) => ", player2_name);
    }

    return safe_three_integers_input(level, b_pos, b_value);
}

static int validate_input_range(int level, int b_pos, int b_value)
{
    if (validate_level(level) != SUCCESS)
    {
        return FAILURE;
    }

    if (validate_b_pos(level, b_pos) != SUCCESS)
    {
        return FAILURE;
    }

    if (validate_b_value(level, b_pos, b_value) != SUCCESS)
    {
        return FAILURE;
    }

    return SUCCESS;
}

static int validate_level(int level)
{
    if ((level > BOARD_SIZE) || (level < 1))
    {
        printf("level 값의 범위가 1~%d가 아닙니다..[현재 level : %d]\n",
            BOARD_SIZE, level);
        return FAILURE;
    }
    return SUCCESS;
}

static int validate_b_pos(int level, int b_pos)
{
    if ((b_pos > BOARD_SIZE) || (b_pos < 1))
    {
        printf("b_pos 값의 범위가 1~%d가 아닙니다...[현재 b_pos : %d]\n",
            BOARD_SIZE, b_pos);
        return FAILURE;
    }

    if (b_pos > level)
    {
        printf("level=%d 에 따른 b_pos=%d 의 값이 적절하지 않습니다..\n",
            level, b_pos);
        return FAILURE;
    }

    return SUCCESS;
}

static int validate_b_value(int level, int b_pos, int b_value)
{
    if (b_value < 1)
    {
        printf("b_value는 1 이상이어야 합니다. [현재 b_value : %d]\n", b_value);
        return FAILURE;
    }

    if ((b_pos + b_value - 1) > level)
    {
        printf("level=%d 과 b_pos=%d 에 따른 b_value=%d 값이 맞지 않습니다..\n",
            level, b_pos, b_value);
        return FAILURE;
    }

    return SUCCESS;
}

static int check_cell_availability(int level, int b_pos, int b_value)
{
    int c = 0;
    int start_pos = b_pos - 1;
    int end_pos = start_pos + b_value - 1;

    for (c = start_pos; c <= end_pos; c++)
    {
        if (board[level - 1][c] != EMPTY_CELL)
        {
            printf("중간에 다른 값이 있습니다. 다시 입력하세요\n");
            return FAILURE;
        }
    }

    return SUCCESS;
}

static void place_symbols(int level, int b_pos, int b_value, int current_turn)
{
    int c = 0;
    int start_pos = b_pos - 1;
    int end_pos = start_pos + b_value - 1;
    char symbol = EMPTY_CELL;

    if (current_turn == FALSE)
    {
        symbol = PLAYER1_SYMBOL;
    }
    else
    {
        symbol = PLAYER2_SYMBOL;
    }

    for (c = start_pos; c <= end_pos; c++)
    {
        board[level - 1][c] = symbol;
    }
}

static int count_remaining_cells(void)
{
    int count = 0;
    int i = 0;
    int j = 0;

    for (i = 0; i < BOARD_SIZE; i++)
    {
        for (j = 0; j < (i + 1); j++)
        {
            if (board[i][j] == EMPTY_CELL)
            {
                count++;
            }
        }
    }

    return count;
}

static void display_game_result(int current_turn)
{
    printf("\n<<게임 종료!!>>\n");

    if (current_turn == FALSE)
    {
        printf("<<%s 승리!!>>\n", player1_name);
    }
    else
    {
        printf("<<%s 승리!!>>\n", player2_name);
    }
}

static void display_player_info(void)
{
    printf("[%s : %c] [%s : %c]\n",
        player1_name, PLAYER1_SYMBOL,
        player2_name, PLAYER2_SYMBOL);
}

static void display_separator(void)
{
    printf("\n===============================================================\n");
}

static void display_game_header(void)
{
    printf("===============================================================\n");
    printf("              삼각형 보드 게임에 오신 것을 환영합니다!            \n");
    printf("===============================================================\n");
    printf("게임 규칙:\n");
    printf("1. 두 플레이어가 번갈아 가며 삼각형 보드에 말을 놓습니다.\n");
    printf("2. level(1-5), b_pos(1-level), b_value(연속 배치할 개수)를 입력합니다.\n");
    printf("3. 모든 칸이 채워지면 마지막에 놓은 플레이어가 승리합니다.\n");
    printf("===============================================================\n");
}
