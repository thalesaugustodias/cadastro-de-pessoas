import { extendTheme } from '@chakra-ui/react';

// Paleta de cores moderna e elegante
const colors = {
    brand: {
        50: '#e8f4fd',
        100: '#bee1fa',
        200: '#90cdf6',
        300: '#5db8f1',
        400: '#3ba6ed',
        500: '#1e77f3', // Cor principal
        600: '#1a6dd4',
        700: '#155eb5',
        800: '#114f96',
        900: '#0c3d6f',
    },
    accent: {
        50: '#e8fffe',
        100: '#c1fcfb',
        200: '#9af8f7',
        300: '#73f4f2',
        400: '#4cf0ee',
        500: '#26ece9', // Verde agua elegante
        600: '#1fbcba',
        700: '#188d8c',
        800: '#115e5d',
        900: '#0a2f2f',
    },
    gray: {
        50: '#f9fafb',
        100: '#f3f4f6',
        200: '#e5e7eb',
        300: '#d1d5db',
        400: '#9ca3af',
        500: '#6b7280',
        600: '#4b5563',
        700: '#374151',
        800: '#1f2937',
        900: '#111827',
    },
    success: {
        50: '#ecfdf5',
        100: '#d1fae5',
        200: '#a7f3d0',
        300: '#6ee7b7',
        400: '#34d399',
        500: '#10b981',
        600: '#059669',
        700: '#047857',
        800: '#065f46',
        900: '#064e3b',
    },
    warning: {
        50: '#fffbeb',
        100: '#fef3c7',
        200: '#fde68a',
        300: '#fcd34d',
        400: '#fbbf24',
        500: '#f59e0b',
        600: '#d97706',
        700: '#b45309',
        800: '#92400e',
        900: '#78350f',
    },
    danger: {
        50: '#fef2f2',
        100: '#fee2e2',
        200: '#fecaca',
        300: '#fca5a5',
        400: '#f87171',
        500: '#ef4444',
        600: '#dc2626',
        700: '#b91c1c',
        800: '#991b1b',
        900: '#7f1d1d',
    }
};

// Tipografia moderna
const fonts = {
    heading: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
    body: "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
};

// Configurações de tamanhos
const sizes = {
    container: {
        sm: '640px',
        md: '768px',
        lg: '1024px',
        xl: '1280px',
    }
};

// Configurações de sombras elegantes
const shadows = {
    xs: '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
    sm: '0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px 0 rgba(0, 0, 0, 0.06)',
    base: '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
    md: '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
    lg: '0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04)',
    xl: '0 25px 50px -12px rgba(0, 0, 0, 0.25)',
    elegant: '0 4px 25px 0 rgba(30, 119, 243, 0.15)',
    cardHover: '0 10px 40px 0 rgba(30, 119, 243, 0.1)',
};

const theme = extendTheme({
    colors,
    fonts,
    sizes,
    shadows,
    config: {
        initialColorMode: 'light',
        useSystemColorMode: false,
    },
    styles: {
        global: {
            body: {
                bg: 'gray.50',
                color: 'gray.800',
                fontFamily: 'body',
                lineHeight: 'base',
            },
            '*::placeholder': {
                color: 'gray.400',
            },
            '*, *::before, &::after': {
                borderColor: 'gray.200',
            },
        },
    },
    components: {
        // Botões modernos
        Button: {
            baseStyle: {
                fontWeight: '600',
                borderRadius: 'lg',
                transition: 'all 0.2s cubic-bezier(.08,.52,.52,1)',
            },
            sizes: {
                xs: {
                    h: '8',
                    minW: '8',
                    fontSize: 'xs',
                    px: '3',
                },
                sm: {
                    h: '10',
                    minW: '10',
                    fontSize: 'sm',
                    px: '4',
                },
                md: {
                    h: '12',
                    minW: '12',
                    fontSize: 'md',
                    px: '6',
                },
                lg: {
                    h: '14',
                    minW: '14',
                    fontSize: 'lg',
                    px: '8',
                },
            },
            variants: {
                primary: {
                    bg: 'brand.500',
                    color: 'white',
                    boxShadow: '0 4px 12px 0 rgba(30, 119, 243, 0.3)',
                    _hover: {
                        bg: 'brand.600',
                        boxShadow: '0 6px 20px 0 rgba(30, 119, 243, 0.4)',
                        transform: 'translateY(-2px)',
                    },
                    _active: {
                        bg: 'brand.700',
                        transform: 'translateY(0)',
                    },
                },
                secondary: {
                    bg: 'accent.500',
                    color: 'white',
                    boxShadow: '0 4px 12px 0 rgba(38, 236, 233, 0.3)',
                    _hover: {
                        bg: 'accent.600',
                        boxShadow: '0 6px 20px 0 rgba(38, 236, 233, 0.4)',
                        transform: 'translateY(-2px)',
                    },
                },
                outline: {
                    border: '2px solid',
                    borderColor: 'brand.500',
                    color: 'brand.500',
                    bg: 'transparent',
                    _hover: {
                        bg: 'brand.50',
                        transform: 'translateY(-2px)',
                        boxShadow: 'md',
                    },
                },
                ghost: {
                    bg: 'transparent',
                    _hover: {
                        bg: 'gray.100',
                        transform: 'translateY(-1px)',
                    },
                },
                danger: {
                    bg: 'danger.500',
                    color: 'white',
                    boxShadow: '0 4px 12px 0 rgba(239, 68, 68, 0.3)',
                    _hover: {
                        bg: 'danger.600',
                        boxShadow: '0 6px 20px 0 rgba(239, 68, 68, 0.4)',
                        transform: 'translateY(-2px)',
                    },
                },
                success: {
                    bg: 'success.500',
                    color: 'white',
                    boxShadow: '0 4px 12px 0 rgba(16, 185, 129, 0.3)',
                    _hover: {
                        bg: 'success.600',
                        boxShadow: '0 6px 20px 0 rgba(16, 185, 129, 0.4)',
                        transform: 'translateY(-2px)',
                    },
                },
            },
        },
        
        // Cards elegantes
        Card: {
            baseStyle: {
                p: '6',
                bg: 'white',
                borderRadius: 'xl',
                boxShadow: 'base',
                border: '1px solid',
                borderColor: 'gray.100',
                transition: 'all 0.3s ease',
                _hover: {
                    boxShadow: 'cardHover',
                    transform: 'translateY(-4px)',
                },
            },
            variants: {
                elevated: {
                    boxShadow: 'lg',
                    _hover: {
                        boxShadow: 'xl',
                        transform: 'translateY(-6px)',
                    },
                },
                outline: {
                    boxShadow: 'none',
                    border: '2px solid',
                    borderColor: 'gray.200',
                    _hover: {
                        borderColor: 'brand.300',
                        boxShadow: 'elegant',
                    },
                },
            },
        },

        // Inputs modernos
        Input: {
            baseStyle: {
                field: {
                    transition: 'all 0.2s',
                    borderRadius: 'lg',
                    border: '2px solid',
                    borderColor: 'gray.200',
                    _hover: {
                        borderColor: 'gray.300',
                    },
                    _focus: {
                        borderColor: 'brand.500',
                        boxShadow: '0 0 0 1px rgba(30, 119, 243, 0.3)',
                    },
                    _invalid: {
                        borderColor: 'danger.500',
                        boxShadow: '0 0 0 1px rgba(239, 68, 68, 0.3)',
                    },
                },
            },
            sizes: {
                lg: {
                    field: {
                        h: '14',
                        px: '4',
                        fontSize: 'lg',
                    },
                },
            },
        },

        // FormLabel melhorado
        FormLabel: {
            baseStyle: {
                fontSize: 'sm',
                fontWeight: '600',
                color: 'gray.700',
                mb: '2',
            },
        },

        // Select moderno
        Select: {
            baseStyle: {
                field: {
                    transition: 'all 0.2s',
                    borderRadius: 'lg',
                    border: '2px solid',
                    borderColor: 'gray.200',
                    _hover: {
                        borderColor: 'gray.300',
                    },
                    _focus: {
                        borderColor: 'brand.500',
                        boxShadow: '0 0 0 1px rgba(30, 119, 243, 0.3)',
                    },
                },
            },
        },

        // Headings elegantes
        Heading: {
            baseStyle: {
                fontWeight: '700',
                letterSpacing: '-0.025em',
            },
            sizes: {
                xs: {
                    fontSize: 'lg',
                    lineHeight: '1.5',
                },
                sm: {
                    fontSize: 'xl',
                    lineHeight: '1.4',
                },
                md: {
                    fontSize: '2xl',
                    lineHeight: '1.3',
                },
                lg: {
                    fontSize: '3xl',
                    lineHeight: '1.2',
                },
                xl: {
                    fontSize: '4xl',
                    lineHeight: '1.1',
                },
                '2xl': {
                    fontSize: '5xl',
                    lineHeight: '1',
                },
            },
        },

        // Tabs modernos
        Tabs: {
            variants: {
                modern: {
                    tablist: {
                        borderBottom: '2px solid',
                        borderColor: 'gray.200',
                    },
                    tab: {
                        fontWeight: '600',
                        color: 'gray.500',
                        _selected: {
                            color: 'brand.500',
                            borderColor: 'brand.500',
                        },
                        _hover: {
                            color: 'brand.400',
                        },
                    },
                },
            },
        },

        // Modal elegante
        Modal: {
            baseStyle: {
                dialog: {
                    borderRadius: 'xl',
                    boxShadow: 'xl',
                },
            },
        },

        // Table moderna
        Table: {
            variants: {
                modern: {
                    table: {
                        bg: 'white',
                        borderRadius: 'xl',
                        overflow: 'hidden',
                        boxShadow: 'base',
                    },
                    thead: {
                        bg: 'gray.50',
                    },
                    th: {
                        fontWeight: '700',
                        textTransform: 'none',
                        letterSpacing: 'normal',
                        color: 'gray.700',
                        borderColor: 'gray.200',
                    },
                    td: {
                        borderColor: 'gray.100',
                    },
                    tbody: {
                        tr: {
                            _hover: {
                                bg: 'gray.50',
                            },
                        },
                    },
                },
            },
        },

        // Badge moderno
        Badge: {
            baseStyle: {
                borderRadius: 'full',
                fontWeight: '600',
                fontSize: 'xs',
                textTransform: 'none',
            },
            variants: {
                modern: {
                    bg: 'brand.100',
                    color: 'brand.800',
                },
                success: {
                    bg: 'success.100',
                    color: 'success.800',
                },
                warning: {
                    bg: 'warning.100',
                    color: 'warning.800',
                },
                danger: {
                    bg: 'danger.100',
                    color: 'danger.800',
                },
            },
        },
    },
});

export default theme;