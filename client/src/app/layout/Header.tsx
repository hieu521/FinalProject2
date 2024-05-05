import { ShoppingCart } from "@mui/icons-material";
import {
  AppBar,
  Badge,
  IconButton,
  List,
  ListItem,
  Toolbar,
  Typography,
} from "@mui/material";
import { Box } from "@mui/system";
import { Link, NavLink } from "react-router-dom";
import { useAppSelector } from "../store/configureStore";
import SignedInMenu from "./SignedInMenu";
import { useMediaQuery } from '@mui/material';
import { useTheme } from '@mui/system';
const midLinks = [
  { title: "Home", path: "/" },
  { title: "Catalog", path: "/catalog" },
  { title: "Chat", path: "/chat" },
];

const rightLinks = [
  { title: "Login", path: "/login" },
  { title: "Register", path: "/register" },
];

const navStyles = {
  color: "#013328",
  textDecoration: "none",
  typography: "h6",
  "&:hover": {
    color: "grey.500",
  },
  "&.active": {
    color: "text.secondary",
  },
};

const listItemStyles = {
  ...navStyles,
  marginRight: "20px", // Thêm khoảng cách giữa các mục trong danh sách
};

interface Props {
  darkMode: boolean;
  handleThemeChange: () => void;
}

export default function Header({ handleThemeChange, darkMode }: Props) {
  const { basket } = useAppSelector((state) => state.basket);
  const { user } = useAppSelector((state) => state.account);
  const itemCount = basket?.items.reduce((sum, item) => sum + item.quantity, 0);
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('sm'));
  return (
    <AppBar
      sx={{
        backgroundColor: "#ffffff",
      }}
      position="static"
    >
      <Toolbar
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          
        }}
      >
        <Box display="flex" alignItems="center">
          <Typography variant="h6" component={NavLink} to="/" sx={navStyles}>
            GymPro
          </Typography>
        </Box>

        <List sx={{ display: "flex", marginLeft: 0 }}>
          {midLinks.map(({ title, path }) => (
            <ListItem component={NavLink} to={path} key={path} sx={listItemStyles}>
              {title.toUpperCase()}
            </ListItem>
          ))}
          {(user && (user.roles?.includes("Admin"))) && (
            <ListItem component={NavLink} to={"/users"} sx={listItemStyles}>
              USERS
            </ListItem>
          )}
          {user && user.roles?.includes("Admin") && (
            <>
              <ListItem component={NavLink} to={"/inventory"} sx={listItemStyles}>
                INVENTORY
              </ListItem>
              <ListItem component={NavLink} to={"/coupons"} sx={listItemStyles}>
                COUPONS
              </ListItem>
              <ListItem component={NavLink} to={"/dashboard"} sx={listItemStyles}>
                DASHBOARD
              </ListItem>
            </>
          )}
        </List>

        <Box display="flex" alignItems="center">
          <IconButton
            component={Link}
            to="/basket"
            size="large"
            edge="start"
            sx={{ mr: 2 }}
          >
            <Badge badgeContent={itemCount} color="secondary">
              <ShoppingCart sx={{ color: "#013328" }} />
            </Badge>
          </IconButton>

          {user ? (
            <SignedInMenu />
          ) : (
            <List sx={{ display: "flex" }}>
              {rightLinks.map(({ title, path }) => (
                <ListItem
                  component={NavLink}
                  to={path}
                  key={path}
                  sx={navStyles}
                >
                  {title.toUpperCase()}
                </ListItem>
              ))}
            </List>
          )}
        </Box>
      </Toolbar>
    </AppBar>
  );
}
